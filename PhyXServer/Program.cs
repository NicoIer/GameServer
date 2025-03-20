// See https://aka.ms/new-console-template for more information

using System.Numerics;
using MagicPhysX.Toolkit;

Console.WriteLine("Hello, World!");

unsafe
{
    using var physics = new PhysicsSystem(enablePvd: false);
    using var scene = physics.CreateScene();

    var material = physics.CreateMaterial(0, 0, 1);

    var plane = scene.AddStaticPlane(0.0f, 1.0f, 0.0f, 0.0f, new Vector3(0, 0, 0), Quaternion.Identity,
        material);
    var sphere = scene.AddDynamicSphere(1.0f, new Vector3(0.0f, 10.0f, 0.0f), Quaternion.Identity, 1.0f,
        material);
    sphere.WakeUp();
    for (var i = 0; i < 200; i++)
    {
        sphere.velocity += new Vector3(1, 1, 1);
        scene.Update(1.0f / 30.0f);
        var position = sphere.transform.position;
        Console.WriteLine($"{position},{sphere.velocity}");
    }
}