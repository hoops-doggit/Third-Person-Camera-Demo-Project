using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SecondOrderDynamicsAngle))]
public class SecondOrderDynamicsAngleDrawer : PropertyDrawer
{
    private const int GraphHeight = 140;
    private const float SimStep = 1f / 120f;
    private const float BottomLabelHeight = 20f;
    private const float MaxGraphTime = 2f;
    private const float InitialHoldTime = 0.2f;

    private float initialOffset = 0f;
    private float targetAngle = 180f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) + GraphHeight + BottomLabelHeight + 24f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        object targetObj = fieldInfo.GetValue(property.serializedObject.targetObject);
        SecondOrderDynamicsAngle spring = targetObj as SecondOrderDynamicsAngle;

        if (spring == null)
        {
            EditorGUI.LabelField(position, "Cannot draw spring: null");
            return;
        }

        // Draw the default property field
        Rect fieldRect = new Rect(position.x, position.y, position.width,
            EditorGUI.GetPropertyHeight(property, label, true));
        EditorGUI.PropertyField(fieldRect, property, label, true);

        // Initial offset slider
        Rect offsetRect = new Rect(position.x, fieldRect.yMax + 2, position.width, 18);
        initialOffset = EditorGUI.Slider(offsetRect, "Initial Offset", initialOffset, 0f, 360f);

        // Target angle slider
        Rect targetRect = new Rect(position.x, offsetRect.yMax + 2, position.width, 18);
        targetAngle = EditorGUI.Slider(targetRect, "Target Angle", targetAngle, 0f, 360f);

        // Graph rectangle
        Rect graphRect = new Rect(position.x, targetRect.yMax + 2, position.width, GraphHeight);
        EditorGUI.DrawRect(graphRect, new Color(0.15f, 0.15f, 0.15f));

        Handles.color = Color.gray;
        Handles.DrawLine(new Vector3(graphRect.x, graphRect.center.y), new Vector3(graphRect.xMax, graphRect.center.y));
        Handles.DrawLine(new Vector3(graphRect.x, graphRect.yMax), new Vector3(graphRect.x, graphRect.y));

        int steps = Mathf.CeilToInt(MaxGraphTime / SimStep);
        int holdSteps = Mathf.CeilToInt(InitialHoldTime / SimStep);

        // Determine max velocity for scaling
        float maxVelocity = 0f;
        SecondOrderDynamicsAngle velocityTester = new SecondOrderDynamicsAngle(initialOffset, spring.Frequency, spring.DampingRatio, spring.Response);
        for (int i = holdSteps; i <= steps; i++)
        {
            velocityTester.Update(SimStep, targetAngle, true);
            maxVelocity = Mathf.Max(maxVelocity, Mathf.Abs(velocityTester.CurrentVelocity));
        }
        maxVelocity = Mathf.Max(maxVelocity, 0.001f);

        Vector3 prevAngle = Vector3.zero;
        Vector3 prevVelocity = Vector3.zero;

        // Reset spring for graph
        SecondOrderDynamicsAngle graphSpring = new SecondOrderDynamicsAngle(initialOffset, spring.Frequency, spring.DampingRatio, spring.Response);

        for (int i = 0; i <= steps; i++)
        {
            float time = i * SimStep;
            float angle;
            float velocity;

            if (i < holdSteps)
            {
                angle = initialOffset;
                velocity = 0f;
            }
            else
            {
                angle = graphSpring.Update(SimStep, targetAngle, true);
                velocity = graphSpring.CurrentVelocity;
            }

            // Angle coordinates
            float x = Mathf.Lerp(graphRect.x, graphRect.xMax, time / MaxGraphTime);
            float yAngle = Mathf.Lerp(graphRect.yMax, graphRect.y, Mathf.InverseLerp(0f, 360f, angle));
            Vector3 pointAngle = new Vector3(x, yAngle, 0);

            if (i > 0) Handles.color = Color.cyan;
            if (i > 0) Handles.DrawLine(prevAngle, pointAngle);
            prevAngle = pointAngle;

            // Velocity coordinates
            float yVelocity = Mathf.Lerp(graphRect.yMax, graphRect.y, Mathf.InverseLerp(-maxVelocity, maxVelocity, velocity));
            Vector3 pointVelocity = new Vector3(x, yVelocity, 0);

            if (i > 0) Handles.color = Color.red;
            if (i > 0) Handles.DrawLine(prevVelocity, pointVelocity);
            prevVelocity = pointVelocity;
        }

        // X-axis labels
        GUI.Label(new Rect(graphRect.x, graphRect.yMax, 30, BottomLabelHeight), "0s");
        GUI.Label(new Rect(graphRect.center.x - 20, graphRect.yMax, 50, BottomLabelHeight),
            (MaxGraphTime * 0.5f).ToString("F2") + "s");
        GUI.Label(new Rect(graphRect.xMax - 40, graphRect.yMax, 50, BottomLabelHeight), MaxGraphTime.ToString("F2") + "s");

        // Y-axis labels
        GUI.Label(new Rect(graphRect.x + 5, graphRect.y, 50, 20), "360°");
        GUI.Label(new Rect(graphRect.x + 5, graphRect.center.y - 10, 50, 20), "180°");
        GUI.Label(new Rect(graphRect.x + 5, graphRect.yMax - BottomLabelHeight, 50, 20), "0°");

        GUI.Label(new Rect(graphRect.xMax - 40, graphRect.y, 40, 20), "Velocity");
    }
}
