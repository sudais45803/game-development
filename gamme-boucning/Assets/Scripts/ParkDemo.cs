using UnityEngine;

public sealed class ParkDemo : MonoBehaviour
{
    public BallTelemetry orangeBall;
    public BallTelemetry blueBall;
    private GUIStyle title, label, small, button;
    private bool paused;

    private void OnGUI()
    {
        if (title == null)
        {
            title = new GUIStyle(GUI.skin.label) { fontSize = 27, fontStyle = FontStyle.Bold };
            title.normal.textColor = new Color(0.96f, 0.95f, 0.86f);
            label = new GUIStyle(GUI.skin.label) { fontSize = 15 };
            label.normal.textColor = Color.white;
            small = new GUIStyle(label) { fontSize = 12 };
            small.normal.textColor = new Color(0.78f, 0.85f, 0.80f);
            button = new GUIStyle(GUI.skin.button) { fontSize = 14 };
        }
        float scale = Mathf.Min(Screen.width / 1100f, Screen.height / 700f);
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
        GUI.color = new Color(0.08f, 0.16f, 0.13f, 0.94f);
        GUI.DrawTexture(new Rect(22, 22, 345, 212), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(42, 34, 305, 20), "UNITY  /  RIGIDBODY + COLLIDERS", small);
        GUI.Label(new Rect(42, 59, 305, 40), "Park Physics", title);
        GUI.Label(new Rect(42, 103, 305, 24), "Orange  ·  1 kg  ·  bounce 0.86", label);
        GUI.Label(new Rect(42, 128, 305, 24), "Blue      ·  3 kg  ·  bounce 0.45", label);
        GUI.Label(new Rect(42, 154, 305, 20), "Gravity: 9.81 m/s²  ·  Ground: Mesh Collider", small);
        if (GUI.Button(new Rect(42, 188, 147, 28), "Drop balls again", button))
        {
            paused = false;
            Time.timeScale = 1;
            orangeBall.ResetBall();
            blueBall.ResetBall();
        }
        if (GUI.Button(new Rect(200, 188, 147, 28), paused ? "Resume" : "Pause", button))
        {
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
        }
        GUI.Label(new Rect(22, Screen.height / scale - 36, 700, 25),
            $"ORANGE: {orangeBall.Bounces} impacts     BLUE: {blueBall.Bounces} impacts     •     Press Play to simulate physics", small);
    }

    private void OnDisable() { Time.timeScale = 1; }
}
