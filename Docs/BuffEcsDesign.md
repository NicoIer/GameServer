---
title: 从战斗系统中的 Buff 设计聊聊 ECS 拆分
date: 2026-06-26 00:00:00
tags: [GameServer, ECS, Buff, 战斗系统]
---

```text
本文记录一下我对战斗系统中 Buff 模块 ECS 化拆分的思考。
重点不是“Buff 要不要写成 Entity”，而是 Buff 逻辑膨胀之后，如何让状态、触发、效果和结算重新变得可推理。
```

# 前言

Buff 系统在战斗系统里很容易被低估。

一开始需求通常很简单：

- 加一点攻击力
- 减一点移动速度
- 每秒掉一点血
- 持续 3 秒眩晕

这个阶段随便写一个 `Buff` 基类都能跑起来。

```csharp
public abstract class Buff
{
    public virtual void OnApply() {}
    public virtual void OnTick(float deltaTime) {}
    public virtual void OnRemove() {}
}
```

然后一个中毒 Buff：

```csharp
public sealed class PoisonBuff : Buff
{
    public override void OnTick(float deltaTime)
    {
        // 每隔一段时间扣血
    }
}
```

这套方案的问题不是它错了，而是它太容易“看起来没错”。

当 Buff 只影响自己，OOP 的继承和回调写起来很自然。但真实战斗系统里的 Buff 很少这么老实。它会影响伤害、移动、技能、道具、攻击盒、怪物 AI、网络同步，甚至还会影响另一个 Buff 的生命周期。

到了这个阶段，`Buff` 很容易从一个简单的状态对象，变成一个到处伸手的规则对象。

# 先说结论

我现在更倾向于把 Buff 拆成 ECS，但不是为了追求某种架构上的纯粹。

更准确地说：

```text
Buff 实例适合作为 Entity
Buff 的持续状态适合作为 Component
Buff 的触发条件适合作为 Trigger
Buff 触发后的行为适合作为 EffectGroup
具体行为适合作为 Effect
```

也就是：

```text
Buff
  -> Trigger A -> EffectGroup A -> Effect 1, Effect 2
  -> Trigger B -> EffectGroup B -> Effect 3
  -> Trigger C -> EffectGroup C -> Effect 4, Effect 5
```

比如“玩家受到伤害时恢复 10 点生命值”，它不是单纯的 Buff，也不是单纯的效果，而是：

```text
Buff: 受击回血
Trigger: OnDamageTaken
Effect: Heal(10)
```

如果这个 Buff 还有另一个规则：

```text
攻击命中时额外造成 20 点火焰伤害
Buff 结束时在脚下生成攻击盒
```

那它就是：

```text
Buff: 复合触发 Buff
Trigger: OnDamageTaken -> EffectGroup: Heal(10)
Trigger: OnHit -> EffectGroup: Damage(20, Fire)
Trigger: OnExpire -> EffectGroup: SpawnAttackBox(...)
```

这比在一个 Buff 类里写多个 `OnXXX` 回调更容易维护。

# OOP Buff 方案为什么会膨胀

常见 OOP Buff 方案一般会沿着这个方向演化：

```csharp
public abstract class Buff
{
    public virtual void OnApply() {}
    public virtual void OnTick(float deltaTime) {}
    public virtual void OnRemove() {}

    public virtual void OnBeforeDamage(DamageContext context) {}
    public virtual void OnAfterDamage(DamageContext context) {}
    public virtual void OnHit(HitContext context) {}
    public virtual void OnBeHit(HitContext context) {}
    public virtual void OnUseItem(ItemContext context) {}
    public virtual void OnKill(Entity target) {}
}
```

它的设计思路是：战斗流程里发生任何事情，都广播给 Buff，让 Buff 决定自己要不要处理。

这个方案前期非常舒服。因为需求来的时候，你只要加一个回调，或者加一个 Buff 子类。

但它的问题也正好在这里。

## 回调越来越多

最开始只有：

```text
OnApply
OnTick
OnRemove
```

后来变成：

```text
OnBeforeDamage
OnAfterDamage
OnBeforeHit
OnAfterHit
OnUseItem
OnMove
OnCastSkill
OnDead
OnKill
```

每加一个回调，Buff 系统就多了一条潜在的横切路径。

伤害系统要问 Buff。

技能系统要问 Buff。

道具系统要问 Buff。

攻击盒系统也要问 Buff。

最后 Buff 变成了一个战斗系统里的全局插件点。它很灵活，但也很危险，因为任何流程都可能被某个 Buff 改写。

## 子类数量越来越多

中毒、灼烧、流血，本质上都很像：

```text
有持续时间
有 tick 间隔
到点造成伤害
可能受伤害类型和抗性影响
```

但是用继承写，很容易得到：

```text
PoisonBuff
BurnBuff
BleedBuff
StrongPoisonBuff
FirePoisonBuff
```

这些类大部分逻辑类似，区别只是参数和少数分支。

继承体系看起来表达了“类型”，但没有沉淀“能力”。

真正可复用的能力应该是：

```text
持续时间
周期触发
造成伤害
可叠层
刷新时间
```

这些能力在 ECS 里更适合变成组件和效果模板，而不是藏在一堆子类里。

## 特殊逻辑会污染通用流程

举一个更接近战斗系统的需求：

```text
玩家使用某个道具后，间隔 X 秒，在玩家当前位置偏移 offset 的地方生成一个攻击盒子。
攻击盒命中怪物后，怪物掉血，道具耐久下降。
```

如果用 OOP Buff，很容易写成：

```text
Item.OnUse()
  -> Buff.OnUseItem()
      -> Buff.StartTimer()
          -> Buff.SpawnAttackBox()
              -> AttackBox.OnHit()
                  -> Buff.OnAttackBoxHit()
                      -> DamageMonster()
                      -> ReduceItemDurability()
```

这个链路里 Buff 知道太多东西了。

它知道道具使用。

它知道延迟计时。

它知道攻击盒创建。

它知道命中结算。

它还知道道具耐久。

单看这个 Buff，逻辑似乎集中；从系统角度看，它已经跨了太多边界。

# ECS 的思路

ECS 不是把 `Buff` 类改名成 `BuffEntity`。

它真正改变的是思考方式。

OOP 容易问：

```text
这个 Buff 应该有什么方法？
```

ECS 更像是在问：

```text
这个 Buff 当前有哪些事实？
这个 Buff 有哪些触发器？
触发器命中后执行哪些效果组？
哪些系统应该处理这些事实？
```

例如一个周期伤害 Buff，它的事实可能是：

```text
它挂在谁身上
它由谁施加
它什么时候过期
它多久触发一次
它每次触发造成多少伤害
```

对应组件可以是：

```text
BuffOwnerComponent
BuffSourceComponent
BuffDurationComponent
PeriodicTriggerComponent
PeriodicDamageEffectComponent
```

然后系统分别处理：

```text
BuffDurationSystem      处理过期
BuffTriggerSystem       处理触发条件
EffectResolveSystem     处理效果组
DamageResolveSystem     处理伤害结算
BuffCleanupSystem       清理过期 Buff
```

这样做之后，Buff 本身不需要“聪明”。系统才是规则所在。

# Buff、Trigger、Effect 的边界

这里需要先把几个概念讲清楚。

## Buff

Buff 是一个持续存在的状态容器。

它表达：

```text
这个状态挂在谁身上
由谁施加
持续多久
能不能叠层
有哪些触发器
```

所以 Buff 更像一个运行时实体，而不是一个执行逻辑的对象。

## Trigger

Trigger 表示触发时机和触发条件。

比如：

```text
OnApply
OnExpire
OnDamageTaken
OnHit
OnUseItem
OnPeriodicTick
```

一个 Buff 可以有多个 Trigger。

不同 Trigger 可以指向不同 EffectGroup。

## EffectGroup

EffectGroup 表示一组效果。

比如受到伤害时：

```text
EffectGroup = [ Heal(10), AddShield(20) ]
```

攻击命中时：

```text
EffectGroup = [ Damage(20, Fire), AddBuff(Burn) ]
```

Buff 结束时：

```text
EffectGroup = [ SpawnAttackBox(9001), DamageSelf(10) ]
```

EffectGroup 的价值是把“触发条件”和“执行效果”分开。这样同一个效果组可以被不同触发器引用，同一个触发器也可以切换到不同效果组。

## Effect

Effect 是具体行为。

例如：

```text
Heal
Damage
AddBuff
RemoveBuff
SpawnAttackBox
ModifyDurability
AddShield
```

我不建议让 Effect 直接绕过战斗结算。比如 `DamageEffect` 最好生成 `DamageEvent`，再让 `DamageResolveSystem` 统一处理护盾、免伤、暴击、仇恨、统计和同步。

# Buff 作为 Entity

我倾向于让 Buff 实例成为 Entity。

一个 Buff Entity 至少需要这些基础组件：

```text
BuffIdComponent
BuffOwnerComponent
BuffSourceComponent
BuffDurationComponent
BuffStackComponent
```

含义大概是：

```text
BuffIdComponent       来自哪个配置
BuffOwnerComponent    挂在哪个角色身上
BuffSourceComponent   谁施加的
BuffDurationComponent 剩余时间、总时间
BuffStackComponent    层数和最大层数
```

然后可以有两种组织触发器的方式。

## 方式一：Trigger 组件直接挂在 Buff Entity 上

```text
Buff Entity
  BuffOwnerComponent
  BuffDurationComponent
  TriggerOnDamageTakenComponent -> effect_damage_taken_3001
  TriggerOnHitComponent -> effect_hit_3001
  TriggerOnExpireComponent -> effect_expire_3001
```

这种方式简单，适合触发器很少、条件不复杂的 Buff。

## 方式二：Trigger 作为独立子 Entity

```text
Buff Entity
  BuffOwnerComponent
  BuffDurationComponent

Trigger Entity A
  ParentBuffComponent
  TriggerOnDamageTakenComponent
  EffectGroupRefComponent

Trigger Entity B
  ParentBuffComponent
  TriggerOnHitComponent
  EffectGroupRefComponent
```

我更推荐这种方式，尤其是后期触发条件会越来越复杂时。

因为它让模型非常清楚：

```text
一个 Buff 可以有多个 Trigger Entity
一个 Trigger Entity = 一个触发时机 + 一个效果组引用
```

系统也更好写。

```text
DamageTakenTriggerSystem
  查询 DamageTakenEvent
  查询 TriggerOnDamageTakenComponent
  生成 EffectGroupRequest

HitTriggerSystem
  查询 HitEvent
  查询 TriggerOnHitComponent
  生成 EffectGroupRequest

EffectResolveSystem
  执行 EffectGroupRequest 对应的效果
```

这样一个 Buff 有多少触发时机都没关系，不会让 Buff 本体变复杂。

# 从战斗流程拆 System

Buff ECS 化之后，系统拆分会变成主要设计点。

我会先按战斗流程拆，而不是按 Buff 类型拆。

## BuffApplySystem

负责消费施加 Buff 的请求。

请求可以来自技能、道具、攻击盒、怪物 AI：

```text
ApplyBuffRequest
{
    SourceEntity
    TargetEntity
    BuffId
    Level
}
```

`BuffApplySystem` 根据配置创建 Buff Entity，并挂上基础组件和 Trigger Entity。

这一步的重点是：配置决定组件组合和触发器组合，但不直接执行战斗逻辑。

## BuffDurationSystem

它只处理时间。

查询所有带 `BuffDurationComponent` 的 Buff Entity，每帧扣时间。时间到了，生成 `OnBuffExpire` 触发事件，再给 Buff Entity 打上 `BuffRemoveTag`。

它不关心这是中毒还是护盾，也不关心这个 Buff 的来源。

这类系统应该非常单纯，因为它处理的是通用事实。

## BuffStackSystem

叠层是 Buff 系统里很容易写乱的地方。

常见规则有：

```text
重复施加时刷新时间
重复施加时增加层数
每层独立计时
达到最大层后触发额外效果
同组 Buff 互斥
高等级覆盖低等级
```

这些规则如果散在每个 Buff 子类里，后面会很难查。

我更倾向于让 `BuffStackSystem` 统一处理叠层和刷新策略。

Buff 配置只描述策略：

```text
StackPolicy = RefreshDuration
MaxStack = 5
MutexGroup = ElementControl
```

系统负责解释策略。

这样改规则的时候，是改一个统一的叠层系统，而不是到处翻 Buff 子类。

## BuffTriggerSystem

触发型 Buff 是最需要小心的。

比如：

```text
命中时追加伤害
受击时反伤
受到伤害时恢复生命
使用道具后生成攻击盒
闪避后加速
击杀后刷新冷却
```

OOP 里一般会加很多回调。

ECS 里我更倾向于把战斗事件也做成短生命周期 Entity。

例如命中事件：

```text
HitEvent
{
    Attacker
    Target
    AttackBox
}
```

伤害事件：

```text
DamageTakenEvent
{
    Source
    Target
    Amount
    DamageType
}
```

道具使用事件：

```text
UseItemEvent
{
    User
    ItemEntity
    ItemConfigId
}
```

触发系统消费这些事件，再查对应 Trigger Entity，生成 `EffectGroupRequest`。

这样触发逻辑从“对象回调”变成“事件数据流”。

## EffectResolveSystem

`EffectResolveSystem` 消费 `EffectGroupRequest`，把配置里的效果转换成标准战斗事件。

例如：

```text
HealEffect -> HealEvent
DamageEffect -> DamageEvent
SpawnAttackBoxEffect -> SpawnAttackBoxRequest
ModifyDurabilityEffect -> ItemDurabilityCostEvent
```

这里依然不要让效果直接修改所有系统的最终状态。

回血走 `HealResolveSystem`。

伤害走 `DamageResolveSystem`。

攻击盒创建走 `AttackBoxSpawnSystem`。

道具耐久走 `ItemDurabilitySystem`。

这样可以保证结算路径统一。

# 一个完整例子：多触发 Buff

假设有一个 Buff：

```text
持续 10 秒
受到伤害时：恢复 10 点生命
攻击命中时：额外造成 20 点火焰伤害
Buff 结束时：在脚下生成一个攻击盒
```

如果用 OOP，大概率会变成：

```csharp
public sealed class ComplexBuff : Buff
{
    public override void OnBeHit(HitContext context) {}
    public override void OnHit(HitContext context) {}
    public override void OnRemove() {}
}
```

这个类会同时知道受击、命中、Buff 移除、回血、伤害、攻击盒创建。以后再加一个触发条件，它会继续变胖。

ECS 下可以这样拆：

```text
Buff Entity
  BuffIdComponent
  BuffOwnerComponent
  BuffDurationComponent

Trigger Entity A
  ParentBuffComponent
  TriggerOnDamageTakenComponent
  EffectGroupRef = effect_damage_taken_4001

Trigger Entity B
  ParentBuffComponent
  TriggerOnHitComponent
  EffectGroupRef = effect_hit_4001

Trigger Entity C
  ParentBuffComponent
  TriggerOnExpireComponent
  EffectGroupRef = effect_expire_4001
```

效果组：

```text
effect_damage_taken_4001
  Heal(10)

effect_hit_4001
  Damage(20, Fire)

effect_expire_4001
  SpawnAttackBox(AttackBoxId = 9001, Offset = (0, 0, 1))
```

这样这个 Buff 有多个触发时机也没关系。

Buff 本体只负责生命周期。

Trigger 负责什么时候触发。

EffectGroup 负责触发后执行哪些效果。

Effect 负责表达具体动作。

# 一个完整例子：道具触发延迟攻击盒

再看一个更贴近玩法的需求：

```text
玩家使用道具后，间隔 X 秒，在玩家位置偏移 offset 的地方创建攻击盒。
攻击盒命中怪物时，怪物掉血，道具耐久下降。
```

如果用 ECS，我会拆成几个阶段。

## 1. 道具使用只产生事件

网络请求或玩家输入进来后，不要直接在 handler 里创建攻击盒。

先生成：

```text
UseItemEvent
{
    UserEntity
    ItemEntity
}
```

这代表“发生了使用道具这件事”。

## 2. Buff 或道具效果消费事件

如果某个 Buff 监听道具使用，它不是通过 `OnUseItem` 回调执行，而是由系统查询：

```text
UseItemEvent
TriggerOnUseItemComponent
```

匹配后，系统生成对应 `EffectGroupRequest`。效果组里可以包含一个延迟攻击盒效果：

```text
DelayedSpawnAttackBoxEffect
{
    DelayMs
    Offset
    AttackBoxId
    SourceItemEntity
}
```

## 3. 延迟系统创建攻击盒

`DelayedAttackBoxSpawnSystem` 到时间后创建攻击盒 Entity：

```text
AttackBoxComponent
OwnerComponent
PositionComponent
DamageComponent
SourceItemComponent
LifetimeComponent
```

这里要注意，攻击盒本身也应该是 Entity。

它不是 Buff 内部的一个临时对象，而是战斗世界里的一个实体。这样碰撞、命中、同步、生命周期都可以走统一系统。

## 4. 命中系统生成结果事件

`AttackBoxHitSystem` 负责检测攻击盒命中了谁。

命中后生成：

```text
HitEvent
DamageEvent
ItemDurabilityCostEvent
```

## 5. 结算系统各做各的事

`DamageResolveSystem` 只负责扣血。

`ItemDurabilitySystem` 只负责扣耐久。

`AttackBoxCleanupSystem` 只负责清理过期攻击盒。

这条链路比 OOP 回调长一点，但每一步的职责都很清楚。

最重要的是：没有一个类需要知道完整故事。

# Excel 配置表怎么配合 ECS Buff

很多项目里的 Buff 都是 Excel 配置驱动。这里最容易踩的坑是：Excel 表到底是在配置“一个 Buff 类”，还是在配置“一个 Buff 拥有哪些能力、触发器和效果组”。

如果是 OOP 思路，表里经常会出现这种字段：

```text
BuffId
BuffClassName
Param1
Param2
Param3
```

例如：

```text
BuffId = 1001
BuffClassName = PoisonBuff
Param1 = 5000
Param2 = 1000
Param3 = 100
```

这类配置前期很快。程序看到 `PoisonBuff`，反射或者 switch 创建对应类，再把参数塞进去。

但问题也很明显：Excel 配置和代码类强绑定。

一旦策划想要一个“持续 5 秒、每秒掉血、同时减速 30%、结束时爆炸”的 Buff，就会开始纠结：

```text
这是 PoisonBuff？
还是 SlowBuff？
还是新写一个 PoisonSlowExplodeBuff？
Param1/Param2/Param3 分别是什么意思？
这个类支不支持结束时触发？
```

最后表会变成两种坏方向之一。

第一种是类越来越多：

```text
PoisonBuff
SlowBuff
PoisonSlowBuff
PoisonExplodeBuff
PoisonSlowExplodeBuff
```

第二种是参数越来越泛：

```text
Param1
Param2
Param3
Param4
ExtJson
CustomArgs
```

这两种本质上都是一个问题：配置表在给 OOP 类喂参数，而不是在描述 Buff 的能力组合。

## ECS 下 Excel 应该配置什么

ECS 思路下，Excel 不应该直接配置“创建哪个 Buff 类”，而应该配置“这个 Buff 的基础信息、触发器和效果组”。

我觉得比较贴近 ECS 的方式，是把配置拆成几层：

```text
Buff 表
  描述 Buff 的身份，以及引用了哪些组件和触发器

BuffComponent 表
  每一行描述一个持续存在的组件模板

BuffTrigger 表
  每一行描述一个触发器，并引用一个效果组

EffectGroup 表
  每一行描述一组效果

Effect 表
  每一行描述一个具体效果
```

Buff 表不直接展开所有字段，而是通过数组字段引用其他表。

```text
BuffId
Name
Tags
ComponentIds
TriggerIds
```

比如：

```text
BuffId = 4001
Name = 复合触发 Buff
ComponentIds = [
    "duration_10s",
    "stack_refresh_5"
]
TriggerIds = [
    "trigger_damage_taken_4001",
    "trigger_hit_4001",
    "trigger_expire_4001"
]
```

Component 表：

```text
ComponentId
ComponentType
参数列
```

例如：

```text
ComponentId = duration_10s
ComponentType = BuffDuration
DurationMs = 10000

ComponentId = stack_refresh_5
ComponentType = BuffStack
MaxStack = 5
StackPolicy = RefreshDuration
```

Trigger 表：

```text
TriggerId
TriggerType
Condition
EffectGroupId
```

例如：

```text
TriggerId = trigger_damage_taken_4001
TriggerType = OnDamageTaken
EffectGroupId = effect_damage_taken_4001

TriggerId = trigger_hit_4001
TriggerType = OnHit
EffectGroupId = effect_hit_4001

TriggerId = trigger_expire_4001
TriggerType = OnBuffExpire
EffectGroupId = effect_expire_4001
```

EffectGroup 表：

```text
EffectGroupId
EffectIds
```

例如：

```text
EffectGroupId = effect_damage_taken_4001
EffectIds = [ "heal_10_4001" ]

EffectGroupId = effect_hit_4001
EffectIds = [ "extra_fire_damage_4001" ]

EffectGroupId = effect_expire_4001
EffectIds = [ "spawn_attack_box_4001" ]
```

Effect 表：

```text
EffectId
EffectType
Param1
Param2
Param3
```

也可以按 `EffectType` 做强类型列：

```text
EffectId = heal_10_4001
EffectType = Heal
Value = 10

EffectId = extra_fire_damage_4001
EffectType = Damage
DamageType = Fire
Value = 20

EffectId = spawn_attack_box_4001
EffectType = SpawnAttackBox
AttackBoxId = 9001
OffsetX = 0
OffsetY = 0
OffsetZ = 1
```

如果 Buff 有持续属性，比如减速、护盾、控制状态，也可以通过 `ComponentIds` 引用组件模板。

```text
ComponentId = slow_move_1001
ComponentType = AttributeModifier
AttributeType = MoveSpeed
ModifyType = PercentAdd
Value = -0.3
```

运行时创建的不是 `ComplexBuff`，而是：

```text
Buff Entity
  BuffIdComponent
  BuffOwnerComponent
  BuffDurationComponent
  BuffStackComponent

Trigger Entity A
  TriggerOnDamageTakenComponent
  EffectGroupRefComponent

Trigger Entity B
  TriggerOnHitComponent
  EffectGroupRefComponent

Trigger Entity C
  TriggerOnExpireComponent
  EffectGroupRefComponent
```

也就是说，Excel 不是在描述一个复杂 Buff 类，而是在描述：

```text
Buff 4001 = 身份信息 + 多个组件模板 + 多个 Trigger + 多个 EffectGroup + 多个 Effect
```

`BuffApplySystem` 创建 Buff Entity 时，根据 `TriggerIds` 创建 Trigger Entity，根据 `ComponentIds` 添加持续组件。触发发生后，系统再根据 `EffectGroupId` 找到效果组并执行。

这里最重要的原则是：运行时真相只有一份。

`DurationMs`、`MaxStack`、`StackPolicy` 这类字段不要同时存在于 Buff 表和 Component 表里。否则早晚会出现：

```text
Buff 表 DurationMs = 10000
Component 表 duration_10s.DurationMs = 8000
```

到底听谁的？

追求架构干净时，答案应该很明确：

```text
Buff 表只描述组合
BuffComponent 表描述持续状态
运行时只根据 ComponentIds 实例化组件
```

如果为了策划扫表方便，确实想在 Buff 表上保留 `DurationMs`、`MaxStack` 这种快捷列，也应该把它们定义成编辑期语法糖：导表时自动生成对应 `BuffComponent`，最终运行时 `BuffConfig` 里仍然只有 `ComponentIndices`。

也就是说，运行时不要同时读两套字段。

这个结构有几个好处：

- Buff 表保持简洁，不会因为触发时机越来越多而不断加列；
- 一个 Buff 可以有多个触发时机，每个时机可以绑定不同效果；
- EffectGroup 可以复用；
- Effect 可以复用；
- 程序侧可以按 `TriggerType`、`EffectType` 做强类型校验和生成；
- 新增一种触发条件或效果时，通常是新增 Trigger/Effect 类型，而不是新增一个 Buff 子类。

这里要注意，表里的 `TriggerType` 和 `EffectType` 不一定要直接暴露 C# 类名。它们可以是策划语义上的类型：

```text
OnDamageTaken
OnHit
OnBuffExpire
Heal
Damage
SpawnAttackBox
```

导入器再把它们映射到运行时组件或事件：

```text
OnDamageTaken -> TriggerOnDamageTakenComponent
Heal -> HealEffect
Damage -> DamageEffect
SpawnAttackBox -> SpawnAttackBoxEffect
```

这样 Excel 仍然面向设计语义，而 ECS 仍然面向运行时数据。

## 配合 OOP 时的问题

Excel 配合 OOP 最大的问题，是配置表很容易被类结构绑架。

常见几种症状：

### 1. BuffClassName 泄漏到配置

表里直接填类名，看起来很灵活：

```text
BuffClassName = PoisonBuff
```

但这意味着策划配置依赖程序类名。类名一改，配置就要跟着改。更麻烦的是，Buff 能力的组合边界被类名固定住了。

想组合两个能力时，要么写新类，要么让旧类加分支。

### 2. 参数列语义不稳定

很多 OOP Buff 表会出现：

```text
Arg1
Arg2
Arg3
```

在 `PoisonBuff` 里，`Arg1` 是伤害。

在 `SlowBuff` 里，`Arg1` 是减速比例。

在 `ShieldBuff` 里，`Arg1` 是护盾值。

这会让表结构失去自解释能力。配置越来越依赖文档和口口相传。

### 3. 复杂 Buff 只能写特殊类

一旦出现组合能力：

```text
周期伤害 + 减速 + 命中后叠层 + 结束后爆炸
```

OOP 很容易走向特殊类：

```text
PoisonSlowStackExplodeBuff
```

特殊类越多，通用系统越少。最后不是配置驱动代码，而是配置在选择代码分支。

### 4. 结算路径容易分叉

有的 Buff 子类直接扣血。

有的 Buff 子类调用伤害系统。

有的 Buff 子类先判断护盾。

有的 Buff 子类绕过暴击和免伤。

这些问题在表里看不出来，因为真正的逻辑藏在类里。配置只告诉你用了哪个类，却不能告诉你这个类走了哪条结算路径。

ECS 的做法是让配置产生标准事件：

```text
DamageEvent
HealEvent
SpawnAttackBoxEvent
ItemDurabilityCostEvent
```

具体结算统一交给系统。

## 一个合理的配置流

我更倾向于这样的流程：

```text
Excel
  Buff 表引用组件模板和触发器

配置生成代码
  生成 BuffConfig、BuffComponentTemplate、BuffTriggerConfig、EffectGroupConfig、EffectConfig

BuffApplySystem
  根据 BuffConfig 创建 Buff Entity 和 Trigger Entity

BuffTriggerSystem
  根据战斗事件触发 EffectGroupRequest

EffectResolveSystem
  根据 EffectGroupConfig 执行 Effect

ECS Systems
  根据标准事件推进战斗逻辑
```

例如：

```text
BuffConfig
{
    BuffId
    ComponentRefs[]
    TriggerRefs[]
}

BuffComponentTemplate
{
    ComponentId
    ComponentType
    Parameters
}

BuffTriggerConfig
{
    TriggerId
    TriggerType
    Condition
    EffectGroupId
}

EffectGroupConfig
{
    EffectGroupId
    EffectRefs[]
}

EffectConfig
{
    EffectId
    EffectType
    Parameters
}
```

运行时不是 switch 到某个 Buff 子类，而是类似：

```text
创建 Buff 基础组件
遍历 BuffConfig.ComponentRefs，添加持续组件
遍历 BuffConfig.TriggerRefs，创建 Trigger Entity
触发器命中后生成 EffectGroupRequest
EffectResolveSystem 遍历 EffectGroupConfig.EffectRefs
根据 EffectType 生成标准战斗事件
```

这样配置表和 ECS 的关系就清楚了：

```text
Excel 负责表达策划意图
Config 负责承载强类型数据
BuffApplySystem 负责实例化 Buff 和 Trigger
BuffTriggerSystem 负责把战斗事件匹配到效果组
EffectResolveSystem 负责把效果组转换为标准事件
ECS System 负责执行最终规则
```

# 工程落地：配置和运行时不要混在一起

为了让这个方案真的能实现，需要先把配置数据和运行时组件分开。

配置数据是静态的：

```text
BuffConfig
BuffComponentTemplate
BuffTriggerConfig
EffectGroupConfig
EffectConfig
```

运行时组件是动态的：

```text
BuffInstanceComponent
BuffOwnerComponent
BuffDurationComponent
TriggerOnDamageTakenComponent
EffectGroupRequestComponent
DamageEventComponent
HealEventComponent
```

这两层不要混。

不要把整份 `BuffConfig` 直接塞到 Entity 上，也不要让战斗系统每帧去解析 Excel 字符串。比较合理的方式是启动时加载配置，并建立索引：

```text
BuffConfigRepository
  GetBuffConfig(buffId)
  GetComponentTemplate(componentIndex)
  GetTriggerConfig(triggerIndex)
  GetEffectGroupConfig(effectGroupIndex)
  GetEffectConfig(effectIndex)
```

Excel 里的字符串 id 可以在加载阶段映射成 int index。战斗帧里尽量使用 int index 或 enum，避免运行时频繁做字符串查找。

比如：

```csharp
public sealed class BuffConfig
{
    public int BuffId;
    public int[] ComponentIndices;
    public int[] TriggerIndices;
}

public sealed class BuffComponentTemplate
{
    public int ComponentIndex;
    public BuffComponentType ComponentType;
    public BuffComponentParameterSet Parameters;
}

public sealed class BuffTriggerConfig
{
    public int TriggerIndex;
    public BuffTriggerType TriggerType;
    public int EffectGroupIndex;
}

public sealed class EffectGroupConfig
{
    public int EffectGroupIndex;
    public int[] EffectIndices;
}

public sealed class EffectConfig
{
    public int EffectIndex;
    public EffectType EffectType;
    public EffectParameterSet Parameters;
}
```

如果项目里有配置代码生成工具，可以进一步把 `EffectParameterSet` 变成强类型结构。早期也可以用参数字典先跑通，但要把解析集中在配置层或 resolver 里，不要散在各个 System。

# 工程落地：运行时 Component 草案

Buff Entity 的基础组件可以先保持很小：

```csharp
public struct BuffInstanceComponent : IComponent
{
    public int BuffId;
}

public struct BuffOwnerComponent : IComponent
{
    public Entity Owner;
}

public struct BuffSourceComponent : IComponent
{
    public Entity Source;
}

public struct BuffDurationComponent : IComponent
{
    public int RemainingMs;
    public int TotalMs;
}

public struct BuffStackComponent : IComponent
{
    public int Count;
    public int MaxCount;
}
```

Trigger Entity 可以这样：

```csharp
public struct ParentBuffComponent : IComponent
{
    public Entity Buff;
}

public struct TriggerOnDamageTakenComponent : IComponent
{
}

public struct TriggerOnHitComponent : IComponent
{
}

public struct TriggerOnBuffExpireComponent : IComponent
{
}

public struct EffectGroupRefComponent : IComponent
{
    public int EffectGroupIndex;
}
```

触发后创建短生命周期请求：

```csharp
public struct EffectGroupRequestComponent : IComponent
{
    public int EffectGroupIndex;
    public Entity Source;
    public Entity Target;
    public Entity Buff;
    public Entity TriggerEvent;
}
```

标准事件可以先做几种最小闭环：

```csharp
public struct DamageEventComponent : IComponent
{
    public Entity Source;
    public Entity Target;
    public int Value;
    public DamageType DamageType;
}

public struct HealEventComponent : IComponent
{
    public Entity Source;
    public Entity Target;
    public int Value;
}

public struct SpawnAttackBoxRequestComponent : IComponent
{
    public Entity Owner;
    public int AttackBoxId;
    public Vector3 Offset;
    public int DelayMs;
}
```

这里的设计原则是：

```text
Buff Entity 存持续状态
Trigger Entity 存触发器
EffectGroupRequest Entity 存一次触发请求
DamageEvent / HealEvent / SpawnAttackBoxRequest 存标准战斗事件
```

EffectGroup 本身不需要做成长期 Entity。它更适合作为配置数据。运行时只需要 `EffectGroupRefComponent` 和 `EffectGroupRequestComponent`。

# 工程落地：System 执行顺序

Buff 系统最怕顺序不清楚。一个可实现的顺序可以先这样定：

```text
1. InputToEventSystem
   把网络请求、玩家输入转成 UseItemEvent / CastSkillEvent

2. AttackBoxHitSystem
   生成 HitEvent / DamageEvent

3. PreBuffDamageResolveSystem
   结算直接伤害，生成 DamageTakenEvent

4. BuffApplySystem
   消费 ApplyBuffRequest，创建 Buff Entity 和 Trigger Entity

5. BuffDurationSystem
   扣 Buff 时间，过期时生成 BuffExpireEvent

6. BuffTriggerSystem
   消费 DamageTakenEvent / HitEvent / UseItemEvent / BuffExpireEvent
   匹配 Trigger Entity，生成 EffectGroupRequest

7. EffectResolveSystem
   展开 EffectGroup，生成标准事件或请求

8. PostBuffResolveSystems
   HealResolveSystem / DamageResolveSystem / AttackBoxSpawnSystem / ItemDurabilitySystem

9. CleanupSystem
   清理本帧事件、过期 Buff、临时请求

10. SyncSystem
   从 ECS 状态投影网络同步数据
```

这里把伤害结算拆成 `PreBuff` 和 `PostBuff` 是为了避免歧义。

第一次结算处理技能、攻击盒直接产生的伤害。

Buff 监听 `DamageTakenEvent` 后可能产生新的 `DamageEvent` 或 `HealEvent`。

第二次结算处理 Buff 效果产生的标准事件。

如果项目不希望一帧内有多轮结算，也可以规定 Buff 效果生成的事件下一帧处理。但这个规则必须写清楚，不能靠系统注册顺序碰运气。

# 工程落地：Effect 如何处理

Effect 的运行时处理不建议写成一个巨大的 `switch`，也不建议让 Effect 直接修改目标最终状态。

更稳的做法是：

```text
EffectResolveSystem
  根据 EffectType 找到 EffectResolver
  Resolver 把 EffectConfig 翻译成标准事件
  标准系统统一结算标准事件
```

可以定义一个 resolver 接口：

```csharp
public interface IEffectResolver
{
    void Resolve(in EffectResolveContext context);
}
```

上下文里放这次触发需要的信息：

```csharp
public readonly struct EffectResolveContext
{
    public readonly EntityStore World;
    public readonly EffectConfig EffectConfig;
    public readonly Entity Source;
    public readonly Entity Target;
    public readonly Entity Buff;
    public readonly Entity TriggerEvent;
}
```

然后注册：

```csharp
resolvers[EffectType.Heal] = new HealEffectResolver();
resolvers[EffectType.Damage] = new DamageEffectResolver();
resolvers[EffectType.SpawnAttackBox] = new SpawnAttackBoxEffectResolver();
resolvers[EffectType.ApplyBuff] = new ApplyBuffEffectResolver();
resolvers[EffectType.ModifyDurability] = new ModifyDurabilityEffectResolver();
```

`HealEffectResolver` 不直接改血量，而是创建事件：

```csharp
public sealed class HealEffectResolver : IEffectResolver
{
    public void Resolve(in EffectResolveContext context)
    {
        int value = context.EffectConfig.GetInt("Value");
        context.World.CreateEntity(new HealEventComponent
        {
            Source = context.Source,
            Target = context.Target,
            Value = value,
        });
    }
}
```

`DamageEffectResolver` 创建伤害事件：

```csharp
public sealed class DamageEffectResolver : IEffectResolver
{
    public void Resolve(in EffectResolveContext context)
    {
        int value = context.EffectConfig.GetInt("Value");
        DamageType damageType = context.EffectConfig.GetEnum<DamageType>("DamageType");
        context.World.CreateEntity(new DamageEventComponent
        {
            Source = context.Source,
            Target = context.Target,
            Value = value,
            DamageType = damageType,
        });
    }
}
```

这套结构的重点是：

```text
EffectResolver 只负责“翻译”
最终状态修改交给标准系统
```

这样新增一个效果，不会污染 Buff 系统，也不会绕过伤害、回血、攻击盒、道具耐久这些统一结算路径。

# 工程落地：Effect 扩展分级

不是每新增一种效果，都要新增一个系统。

我会把 Effect 扩展分成四档。

## 1. 只改配置

如果效果语义已经存在，只是数值不同，只需要新增 Excel 行。

例如：

```text
恢复 10 点生命值
恢复 20 点生命值
恢复 MaxHp * 5% 生命值
```

如果 `HealEffectResolver` 已经支持固定值和属性比例，这些都只是配置差异。

代码不应该变化。

## 2. 新增 Resolver，但复用已有事件

如果效果只是用一种新的方式生成已有事件，可以只加 Resolver。

例如：

```text
根据已损失生命值恢复生命
根据攻击者攻击力造成额外伤害
根据 Buff 层数提高治疗量
```

这些最终仍然可以生成：

```text
HealEventComponent
DamageEventComponent
```

所以只需要新增：

```text
EffectType
EffectConfig 字段
EffectResolver
Resolver 注册
Resolver 单测
```

不需要改 `HealResolveSystem` 或 `DamageResolveSystem`。

## 3. 新增请求组件和处理系统

如果效果引入了新的战斗能力，才需要新增请求组件和系统。

例如：

```text
拉拽目标
击退目标
创建延迟攻击盒
召唤单位
修改道具耐久
施加控制状态
```

这类效果应该拆成：

```text
EffectResolver
  把配置翻译成标准请求组件

XxxSystem
  消费请求组件
  处理规则
  修改目标状态
  删除请求 Entity
```

这样 Buff 只是触发来源之一。

同一个 `KnockbackRequestComponent` 可以来自 Buff，也可以来自技能、怪物 AI、场景机关。

## 4. 新增长期状态组件

如果效果不是一次性事件，而是给目标挂一个持续状态，应该落成 Component。

例如：

```text
移动速度降低 20%
受到治疗提高 30%
每秒受到 5 点伤害
无法移动
攻击附带火焰伤害
```

这些不应该在 EffectResolver 里直接修改最终属性。

更好的方式是：

```text
EffectResolver
  创建 AddComponentRequest 或 ApplyBuffRequest

BuffApplySystem / StateApplySystem
  给目标 Entity 添加对应组件

AttributeSystem / DamageSystem / ControlSystem
  在自己的结算阶段读取这些组件
```

持续状态由对应 System 解释，不由 Buff 自己解释。

## Effect 配置字段怎么设计

Excel 里不要长期使用这种字段：

```text
Arg1
Arg2
Arg3
```

短期可以用来过渡，但最终会变成没人知道含义的“魔法列”。

更工程化的方式是：

```text
EffectType 决定参数 Schema
配置导出时校验必填字段
配置导出时把字符串 id 解析成整数索引
运行时只读强类型配置
```

例如：

```text
EffectType = Heal
Value = 10
ValueType = Fixed
TargetSelector = Owner
```

或者：

```text
EffectType = SpawnAttackBox
AttackBoxId = slash_box_001
DelayMs = 500
OffsetX = 2
OffsetY = 0
TargetSelector = Owner
```

`EffectType` 是逻辑类型，字段是这个类型需要的数据。

配置导出阶段应该失败得尽量早：

```text
Heal 缺少 Value，导出失败
SpawnAttackBox 缺少 AttackBoxId，导出失败
TargetSelector 写了不存在的枚举，导出失败
EffectGroup 引用了不存在的 EffectId，导出失败
```

运行时不要再容忍这些错误。

# 工程落地：新增一种 Effect 怎么做

假设要新增一个效果：

```text
把目标拉向施法者 3 米
```

不要新增一个 Buff 子类。

应该按下面步骤做。

## 1. Excel 增加 EffectType

```text
EffectId = pull_to_source_3001
EffectType = PullToSource
Distance = 3
Speed = 10
```

## 2. 配置生成增加强类型字段

如果当前还是通用参数字典，可以先加解析逻辑。

如果已经有强类型生成，可以生成：

```csharp
public sealed class PullToSourceEffectConfig
{
    public float Distance;
    public float Speed;
}
```

## 3. 新增标准请求组件

```csharp
public struct PullRequestComponent : IComponent
{
    public Entity Source;
    public Entity Target;
    public float Distance;
    public float Speed;
}
```

## 4. 新增 EffectResolver

```csharp
public sealed class PullToSourceEffectResolver : IEffectResolver
{
    public void Resolve(in EffectResolveContext context)
    {
        float distance = context.EffectConfig.GetFloat("Distance");
        float speed = context.EffectConfig.GetFloat("Speed");
        context.World.CreateEntity(new PullRequestComponent
        {
            Source = context.Source,
            Target = context.Target,
            Distance = distance,
            Speed = speed,
        });
    }
}
```

## 5. 注册 Resolver

```csharp
resolvers[EffectType.PullToSource] = new PullToSourceEffectResolver();
```

## 6. 新增或复用处理系统

```text
PullSystem
  消费 PullRequestComponent
  根据 Source/Target 位置计算方向
  修改 Target 的位移、速度或控制状态
  删除 PullRequest Entity
```

## 7. 增加测试

至少测三件事：

```text
EffectResolveSystem 能把 PullToSource EffectConfig 转成 PullRequest
PullSystem 能按 Source/Target 生成正确运动状态
这个效果不会绕过控制免疫、霸体、位移限制等通用规则
```

新增效果的成本应该是线性的：

```text
新增配置
新增 EffectType
新增 Resolver
新增标准请求/事件
新增处理 System
新增测试
```

而不是：

```text
新增一个 Buff 子类
改 Buff 基类回调
改多个战斗流程
复制一堆已有逻辑
```

更重要的是，新增 Effect 时通常不应该改这些地方：

```text
BuffTriggerSystem
BuffDurationSystem
BuffStackSystem
已有伤害结算主流程
已有同步主流程
```

如果新增一个 Effect 必须改这些公共系统，通常说明它不是“一个新效果”这么简单，而是在引入新的战斗规则。这个时候应该把它提升成一个明确的新系统，而不是偷偷塞进某个 Buff 分支里。

# 工程落地：第一版 MVP 怎么做

如果要真的在项目里开始做，我不建议第一版就支持所有触发器和所有效果。

第一版只需要跑通一个闭环：

```text
受到伤害时，恢复 10 点生命值
```

这个闭环足够验证：

```text
Buff 创建
Trigger 创建
DamageTakenEvent 触发
EffectGroupRequest 生成
HealEffect 展开
HealEvent 结算
临时事件清理
```

## 需要的配置

Buff 表：

```text
BuffId = 3001
Name = 受击回血
ComponentIds = [ "duration_10s_3001" ]
TriggerIds = [ "trigger_damage_taken_3001" ]
```

Component 表：

```text
ComponentId = duration_10s_3001
ComponentType = BuffDuration
DurationMs = 10000
```

Trigger 表：

```text
TriggerId = trigger_damage_taken_3001
TriggerType = OnDamageTaken
EffectGroupId = effect_damage_taken_3001
```

EffectGroup 表：

```text
EffectGroupId = effect_damage_taken_3001
EffectIds = [ "heal_10_3001" ]
```

Effect 表：

```text
EffectId = heal_10_3001
EffectType = Heal
Value = 10
```

## 需要的组件

```text
BuffInstanceComponent
BuffOwnerComponent
BuffDurationComponent
ParentBuffComponent
TriggerOnDamageTakenComponent
EffectGroupRefComponent
DamageTakenEventComponent
EffectGroupRequestComponent
HealEventComponent
HealthComponent
```

## 需要的系统

```text
BuffApplySystem
  根据 BuffId 创建 Buff Entity 和 Trigger Entity

DamageResolveSystem
  扣血，并生成 DamageTakenEvent

BuffTriggerSystem
  发现 DamageTakenEvent，匹配 TriggerOnDamageTakenComponent，生成 EffectGroupRequest

EffectResolveSystem
  读取 EffectGroupConfig，展开 HealEffect，生成 HealEvent

HealResolveSystem
  消费 HealEvent，恢复 HealthComponent

CleanupSystem
  删除 DamageTakenEvent、EffectGroupRequest、HealEvent
```

## MVP 测试应该覆盖什么

至少要有这些测试：

```text
ApplyBuff 后能创建 Buff Entity
ApplyBuff 后能创建 Trigger Entity
DamageTakenEvent 产生后能匹配到 Trigger
Trigger 能生成正确 EffectGroupRequest
EffectResolveSystem 能把 HealEffect 变成 HealEvent
HealResolveSystem 能恢复 10 点生命值
事件在 CleanupSystem 后被清理
```

这个 MVP 跑通后，再加 `OnHit`、`OnBuffExpire`、`DamageEffect`、`SpawnAttackBoxEffect`。不要一开始就把所有效果都做进去。

# 服务端战斗里的边界

如果是在服务端战斗系统里，我不建议把所有东西都 ECS 化。

这些适合进 ECS：

```text
玩家
怪物
道具
攻击盒
Buff
Trigger
EffectRequest
伤害事件
命中事件
位置
血量
属性
控制状态
```

这些不一定适合：

```text
连接 id
RPC 上下文
房间 id
房间关闭状态
日志对象
PushHub
```

我的理解是：

```text
ECS 负责战斗世界
Room Runtime 负责服务器工程层
```

网络请求进来后，可以先转成 ECS 里的输入事件或命令组件。

ECS 系统在帧内消费这些事件。

同步系统再从 ECS 状态投影出网络消息。

这样 Buff 不需要直接知道连接，也不需要直接调用推送接口。

# ECS 方案的代价

ECS 不是银弹。

它会带来几个明显成本：

- 组件数量会增加
- 系统数量会增加
- 调试方式从看调用栈变成看数据流
- 需要明确系统执行顺序
- 事件 Entity 需要清理
- 对团队代码习惯有要求

所以如果项目只有十几个简单 Buff，OOP 方案完全可以接受。

但如果 Buff 已经开始跨越这些模块：

```text
技能
道具
装备
攻击盒
伤害
怪物 AI
同步
回放
```

那继续用 OOP 回调会越来越难受。

判断是否值得 ECS 化，我会看三个问题：

1. Buff 是否经常由多个能力组合而成？
2. Buff 是否需要响应很多战斗事件？
3. Buff 是否需要统一结算、同步、回放和调试？

如果答案都是 yes，那 ECS 化就不是过度设计。

# 推荐迁移路线

不要一上来推倒重写。

我会按这个顺序迁移：

1. 先让玩家、怪物、攻击盒、Buff 实例成为 Entity。
2. 把 Buff 的 owner、source、duration、stack 抽成基础组件。
3. 把 Trigger 和 EffectGroup 的配置模型定下来。
4. 把周期伤害、属性修改、控制状态先拆成系统。
5. 把伤害事件、命中事件、道具使用事件变成短生命周期 Entity。
6. 再处理触发型 Buff 和互斥叠层规则。
7. 最后把同步从 ECS 状态投影生成。

这样每一步都有收益，也不会要求整个战斗系统一次性换血。

# 总结

OOP Buff 的问题不是“继承不好”，而是 Buff 很容易同时承担太多职责：

```text
状态
规则
事件监听
跨模块调用
结算
同步
```

ECS 的价值，是把这些东西拆回到更清楚的位置：

```text
Entity 表示 Buff 实例
Trigger 表示什么时候触发
EffectGroup 表示触发后执行哪组效果
Effect 表示具体行为
Component 表示持续存在的事实和能力
System 表示规则如何推进
Event Entity 表示战斗里发生了什么
```

从战斗系统角度看，一个好的 Buff 系统不应该追求“每个 Buff 都是一个聪明对象”。

它应该追求：

```text
规则可组合
执行顺序清楚
状态来源明确
结算路径统一
调试能还原数据流
同步能从状态投影
```

当 Buff 开始和道具、攻击盒、伤害、怪物、玩家状态深度交织时，把 Buff 拆进 ECS，不是为了套架构，而是为了让战斗系统重新变得可推理。
