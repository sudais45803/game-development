using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ParkSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/ParkPhysicsEnvironment.unity";

    public static void OpenSavedScene()
    {
        EditorSceneManager.OpenScene(ScenePath);
        EditorApplication.delayCall += () =>
        {
            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.LookAt(new Vector3(0, 1, 0), Camera.main.transform.rotation, 19);
            EditorApplication.ExecuteMenuItem("Window/General/Game");
            Debug.Log("PARK_PROJECT_OPENED: Press Play to watch the two balls bounce.");
        };
    }

    [MenuItem("Park Physics/Rebuild Assignment Scene")]
    public static void Build()
    {
        Directory.CreateDirectory("Assets/Scenes");
        Directory.CreateDirectory("Assets/Materials");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        PlayerSettings.companyName = "Student Projects";
        PlayerSettings.productName = "Park Physics Environment";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 800;
        QualitySettings.shadowDistance = 70;
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowResolution = ShadowResolution.High;
        QualitySettings.antiAliasing = 4;
        Physics.gravity = new Vector3(0, -9.81f, 0);
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.68f, 0.75f, 0.70f);

        var grass = Material("Grass", "70975B");
        var soil = Material("Island Edge", "4B6343");
        var stone = Material("Sandstone Path", "DDD4B9");
        var wood = Material("Bench Wood", "B77742");
        var iron = Material("Bench Legs", "344D43");
        var bark = Material("Tree Bark", "72513B");
        var leaves = Material("Tree Leaves", "477748");
        var lightLeaves = Material("Light Leaves", "7DAD65");
        var orange = Material("Orange Ball", "EF8848");
        var blue = Material("Blue Ball", "579DC0");
        var groundPhysics = PhysicsMaterial("Ground Grip", 0, 0.6f);
        var orangePhysics = PhysicsMaterial("High Bounce - 0.86", 0.86f, 0.15f);
        var bluePhysics = PhysicsMaterial("Low Bounce - 0.45", 0.45f, 0.25f);

        var environment = new GameObject("01 - Park Environment").transform;
        var ground = Primitive(PrimitiveType.Plane, "Ground Plane", environment,
            Vector3.zero, new Vector3(1.8f, 1, 1.4f), grass);
        ground.GetComponent<Collider>().sharedMaterial = groundPhysics;
        Primitive(PrimitiveType.Cube, "Raised Grass Island", environment,
            new Vector3(0, -0.24f, 0), new Vector3(18, 0.45f, 14), soil);
        Primitive(PrimitiveType.Cube, "Central Walking Path", environment,
            new Vector3(0, 0.025f, 0), new Vector3(1.6f, 0.05f, 14), stone);
        Primitive(PrimitiveType.Cube, "Cross Walking Path", environment,
            new Vector3(0, 0.027f, 2), new Vector3(18, 0.05f, 1.4f), stone);

        var benches = new GameObject("02 - Benches (Cube Primitives)").transform;
        Bench("West Bench", benches, new Vector3(-5, 0, 0), 90, wood, iron);
        Bench("East Bench", benches, new Vector3(5, 0, 0), -90, wood, iron);
        var trees = new GameObject("03 - Trees (Cylinder Trunks)").transform;
        Tree("Northwest Tree", trees, new Vector3(-6, 0, 4.8f), bark, leaves);
        Tree("Northeast Tree", trees, new Vector3(6, 0, 4.8f), bark, lightLeaves);
        Tree("Southwest Tree", trees, new Vector3(-7, 0, -4.8f), bark, lightLeaves);
        Tree("Southeast Tree", trees, new Vector3(7, 0, -4.8f), bark, leaves);

        var physics = new GameObject("04 - Physics Objects (Two Spheres)").transform;
        var ballA = Ball("Orange Bouncing Sphere - 1kg", physics, new Vector3(-2.5f, 5, -2.2f), orange, orangePhysics, 1);
        var ballB = Ball("Blue Bouncing Sphere - 3kg", physics, new Vector3(2.5f, 6, -2.2f), blue, bluePhysics, 3);

        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        var camera = cameraObject.GetComponent<Camera>();
        camera.transform.position = new Vector3(17, 17, -22);
        camera.transform.LookAt(new Vector3(0, 1.5f, 0));
        camera.orthographic = true;
        camera.orthographicSize = 12.2f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 100;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.79f, 0.85f, 0.81f);

        var sun = new GameObject("Afternoon Sun", typeof(Light)).GetComponent<Light>();
        sun.type = LightType.Directional;
        sun.color = new Color(1, 0.94f, 0.82f);
        sun.intensity = 1.15f;
        sun.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(48, -35, 0);
        var demo = new GameObject("05 - Demo Controls").AddComponent<ParkDemo>();
        demo.orangeBall = ballA;
        demo.blueBall = ballB;
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.LookAt(new Vector3(0, 1, 0), camera.transform.rotation, 19);
        Debug.Log("PARK_SCENE_CREATED: " + ScenePath);
        Validate();
    }

    private static Material Material(string name, string hex)
    {
        string path = "Assets/Materials/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }
        ColorUtility.TryParseHtmlString("#" + hex, out var color);
        material.color = color;
        material.SetFloat("_Glossiness", 0.2f);
        return material;
    }

    private static PhysicsMaterial PhysicsMaterial(string name, float bounce, float friction)
    {
        string path = "Assets/Materials/" + name + ".physicMaterial";
        var material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
        if (material == null)
        {
            material = new PhysicsMaterial(name);
            AssetDatabase.CreateAsset(material, path);
        }
        material.bounciness = bounce;
        material.dynamicFriction = friction;
        material.staticFriction = friction;
        material.bounceCombine = PhysicsMaterialCombine.Maximum;
        material.frictionCombine = PhysicsMaterialCombine.Average;
        return material;
    }

    private static GameObject Primitive(PrimitiveType type, string name, Transform parent,
        Vector3 position, Vector3 scale, Material material)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static void Bench(string name, Transform parent, Vector3 position, float angle, Material wood, Material iron)
    {
        var root = new GameObject(name).transform;
        root.SetParent(parent, false);
        root.localPosition = position;
        root.localRotation = Quaternion.Euler(0, angle, 0);
        Primitive(PrimitiveType.Cube, "Cube - Seat", root, new Vector3(0, 0.85f, 0), new Vector3(3, 0.18f, 0.95f), wood);
        var back = Primitive(PrimitiveType.Cube, "Cube - Rotated Backrest", root, new Vector3(0, 1.4f, 0.4f), new Vector3(3, 0.95f, 0.16f), wood);
        back.transform.localRotation = Quaternion.Euler(-8, 0, 0);
        Primitive(PrimitiveType.Cube, "Cube - Left Support", root, new Vector3(-1.1f, 0.39f, 0), new Vector3(0.2f, 0.78f, 0.8f), iron);
        Primitive(PrimitiveType.Cube, "Cube - Right Support", root, new Vector3(1.1f, 0.39f, 0), new Vector3(0.2f, 0.78f, 0.8f), iron);
    }

    private static void Tree(string name, Transform parent, Vector3 position, Material bark, Material leaves)
    {
        var root = new GameObject(name).transform;
        root.SetParent(parent, false);
        root.localPosition = position;
        Primitive(PrimitiveType.Cylinder, "Cylinder - Trunk", root, new Vector3(0, 1.15f, 0), new Vector3(0.5f, 1.15f, 0.5f), bark);
        Primitive(PrimitiveType.Sphere, "Sphere - Canopy", root, new Vector3(0, 3, 0), new Vector3(2.5f, 2.8f, 2.5f), leaves);
    }

    private static BallTelemetry Ball(string name, Transform parent, Vector3 position, Material visual, PhysicsMaterial physics, float mass)
    {
        var obj = Primitive(PrimitiveType.Sphere, name, parent, position, Vector3.one * 1.2f, visual);
        obj.GetComponent<SphereCollider>().sharedMaterial = physics;
        var body = obj.AddComponent<Rigidbody>();
        body.mass = mass;
        body.useGravity = true;
        body.isKinematic = false;
        body.linearDamping = 0;
        body.angularDamping = 0.05f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        return obj.AddComponent<BallTelemetry>();
    }

    [MenuItem("Park Physics/Validate Saved Scene")]
    public static void Validate()
    {
        EditorSceneManager.OpenScene(ScenePath);
        var bodies = UnityEngine.Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        if (bodies.Length != 2) throw new Exception("Expected two physics spheres.");
        if (GameObject.Find("Ground Plane").GetComponent<MeshCollider>() == null)
            throw new Exception("Missing ground collider.");
        var initial = bodies.Select(b => b.position).ToArray();
        var bounced = new bool[bodies.Length];
        var oldMode = Physics.simulationMode;
        try
        {
            Physics.simulationMode = SimulationMode.Script;
            Physics.SyncTransforms();
            for (int frame = 0; frame < 600; frame++)
            {
                Physics.Simulate(0.02f);
                for (int i = 0; i < bodies.Length; i++)
                {
                    if (bodies[i].position.y < 0.5f) throw new Exception("Ball fell through ground.");
                    if (bodies[i].linearVelocity.y > 1) bounced[i] = true;
                }
            }
            if (bounced.Any(b => !b)) throw new Exception("A ball did not bounce.");
            Directory.CreateDirectory("Validation");
            File.WriteAllText("Validation/physics-check.txt",
                "PASS: Unity compiled the scripts and saved ParkPhysicsEnvironment.unity.\n" +
                "PASS: Two non-kinematic, gravity-enabled Rigidbody spheres.\n" +
                "PASS: Ground Plane has a MeshCollider.\n" +
                "PASS: Both spheres bounced upwards after falling under gravity.\n" +
                "PASS: Neither sphere fell through the ground during 12 simulated seconds.\n");
            Debug.Log("PARK_PHYSICS_VALIDATION_PASSED");
        }
        finally
        {
            Physics.simulationMode = oldMode;
            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].position = initial[i];
                bodies[i].linearVelocity = Vector3.zero;
                bodies[i].angularVelocity = Vector3.zero;
            }
            EditorSceneManager.OpenScene(ScenePath);
        }
    }

    public static void RenderPreview()
    {
        EditorSceneManager.OpenScene(ScenePath);
        var camera = Camera.main;
        var target = new RenderTexture(1280, 800, 24);
        camera.targetTexture = target;
        camera.Render();
        RenderTexture.active = target;
        var image = new Texture2D(1280, 800, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1280, 800), 0, 0);
        image.Apply();
        Directory.CreateDirectory("Validation");
        File.WriteAllBytes("Validation/park-preview.png", image.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.DestroyImmediate(target);
        UnityEngine.Object.DestroyImmediate(image);
        Debug.Log("PARK_PREVIEW_RENDERED");
    }
}
