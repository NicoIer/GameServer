using System.Numerics;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Raylib_cs;
using MotionType = JoltPhysicsSharp.MotionType;

namespace JoltServer;

[Obsolete]
public class JoltRaylibDebugger : JoltApplication.ISystem
{
    public JoltApplication JoltApplication { get; private set; }

    public Model planeModel;
    public Mesh boxMesh;
    public Material boxMaterial;

    public Camera3D mainCamera;

    private int width;
    private int height;
    private string title;

    public JoltRaylibDebugger(int width, int height, string title, int fps)
    {
        this.width = width;
        this.height = height;
        this.title = title;
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(width, height, title);
        Raylib.SetTargetFPS(fps);
        mainCamera = new Camera3D()
        {
            Position = new Vector3(-20.0f, 8.0f, 10.0f),
            Target = new Vector3(0.0f, 4.0f, 0.0f),
            Up = new Vector3(0.0f, 1.0f, 0.0f),
            FovY = 45.0f,
            Projection = CameraProjection.Perspective
        };
        // dynamically create a plane model
        Texture2D texture = GenCheckedTexture(10, 1, Color.LightGray, Color.Gray);
        Model planeMesh = Raylib.LoadModelFromMesh(Raylib.GenMeshPlane(24, 24, 1, 1));
        Raylib.SetMaterialTexture(ref planeMesh, 0, MaterialMapIndex.Diffuse, ref texture);
        planeModel = planeMesh;

        // dynamically create a box model
        var boxTexture = GenCheckedTexture(2, 1, Color.White, Color.Magenta);
        boxMesh = Raylib.GenMeshCube(1, 1, 1);
        Material boxMat = Raylib.LoadMaterialDefault();
        Raylib.SetMaterialTexture(ref boxMat, MaterialMapIndex.Diffuse, boxTexture);
        boxMaterial = boxMat;
    }

    protected static Texture2D GenCheckedTexture(int size, int checks, Color colorA, Color colorB)
    {
        Image imageMag = Raylib.GenImageChecked(size, size, checks, checks, colorA, colorB);
        Texture2D textureMag = Raylib.LoadTextureFromImage(imageMag);
        Raylib.UnloadImage(imageMag);
        return textureMag;
    }

    public void OnAdded(JoltApplication app)
    {
        JoltApplication = app;
    }

    public void OnRemoved()
    {
    }

    public void BeforeRun()
    {
        JoltApplication.CreateFloor(100, (ushort)ObjectLayers.NonMoving);
    }

    public void AfterRun()
    {
        Raylib.CloseWindow();
    }

    public unsafe void BeforeUpdate(in JoltApplication.LoopContex ctx)
    {
        // 如果点击了鼠标 就添加一个盒子
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            // Mouse To WorldData
            var raylib = Raylib.GetScreenToWorldRay(mousePos, mainCamera);

            var ray = new JoltPhysicsSharp.Ray(raylib.Position, raylib.Direction);
            BroadPhaseLayerFilter broadPhaseLayerFilter = default;
            ObjectLayerFilter objectLayerFilter = default;

            // Span<RayCastResult> results = stackalloc RayCastResult[1] ;
            var results = new RayCastResult[1];
            if (JoltApplication.physicsSystem.NarrowPhaseQuery.CastRay(ray, default, CollisionCollectorType.AnyHit,
                    results))
            {
                var hitPos = ray.Position + ray.Direction * results[0].Fraction;
                _ = JoltApplication.CreateBox(
                    new Vector3(0.5f),
                    hitPos,
                    Quaternion.Identity,
                    MotionType.Dynamic,
                    (ushort)ObjectLayers.Moving);
            }

            // var collision = Raylib.GetRayCollisionBox(ray,
            //     new Raylib_cs.BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1)));
            // if (collision.Hit)
            // {
            //     _ = JoltApplication.CreateBox(
            //         new Vector3(0.5f),
            //         collision.Point,
            //         Quaternion.Identity,
            //         MotionType.Dynamic,
            //         JoltApplication.Layers.Moving);
            //
            //     // new Vector3(0.5f), hitPos, Quaternion.Identity, 1.0f);
            // }
        }
    }

    private Dictionary<Vector3, Raylib_cs.Mesh> _cachedMeshes = new();

    public void AfterUpdate(in JoltApplication.LoopContex ctx)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Blue);
        Raylib.BeginMode3D(mainCamera);
        // Raylib.DrawModel(planeModel, Vector3.Zero, 1.0f, Color.White);

        foreach (BodyID bodyID in JoltApplication.bodies)
        {
            // if (JoltApplication.ignoreDrawBodies.Contains(bodyID))
            //     continue;

            //Vector3 pos = BodyInterface.GetPosition(bodyID);
            //Quaternion rot = BodyInterface.GetRotation(bodyID);
            //Matrix4x4 ori = Matrix4x4.CreateFromQuaternion(rot);
            //Matrix4x4 matrix = new(
            //    ori.M11, ori.M12, ori.M13, pos.X,
            //    ori.M21, ori.M22, ori.M23, pos.Y,
            //    ori.M31, ori.M32, ori.M33, pos.Z,
            //    0, 0, 0, 1.0f);

            // Raylib uses column major matrix
            Matrix4x4 worldTransform = JoltApplication.physicsSystem.BodyInterface.GetWorldTransform(bodyID);
            var shape = JoltApplication.physicsSystem.BodyInterface.GetShape(bodyID);
            if (shape is BoxShape boxShape)
            {
                Mesh mesh;
                if (_cachedMeshes.TryGetValue(boxShape.HalfExtent, out var cachedMesh))
                {
                    mesh = cachedMesh;
                }
                else
                {
                    mesh = Raylib.GenMeshCube(boxShape.HalfExtent.X * 2, boxShape.HalfExtent.Y * 2,
                        boxShape.HalfExtent.Z * 2);
                    _cachedMeshes[boxShape.HalfExtent] = mesh;
                }

                Raylib.DrawMesh(mesh, boxMaterial, Matrix4x4.Transpose(worldTransform));
            }

            // Matrix4x4 drawTransform = Matrix4x4.Transpose(worldTransform);
            // Raylib.DrawMesh(boxMesh, boxMaterial, drawTransform);
        }

        Raylib.EndMode3D();
        Raylib.DrawText($"{Raylib.GetFPS()} fps", 10, 10, 20, Color.White);
        Raylib.EndDrawing();
    }


    public bool NeedShutdown()
    {
        return Raylib.WindowShouldClose();
    }

    public void Dispose()
    {
    }
}