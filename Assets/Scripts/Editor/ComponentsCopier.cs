#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class ComponentsCopier
{

    static Component[] copiedComponents;

    [MenuItem("Tools/GameObject/Copy highLight components %&C")]
    static void Copy()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return;

        var comp1 = UnityEditor.Selection.activeGameObject.GetComponent<HighlightPlus.HighlightEffect>();
        var comp2 = UnityEditor.Selection.activeGameObject.GetComponent<HighlightPlus.HighlightTrigger>();
        var comp3 = UnityEditor.Selection.activeGameObject.GetComponent<Collider>();
        //copiedComponents = UnityEditor.Selection.activeGameObject.GetComponents<Component>();
        if (comp1 == null && comp2 == null && comp3 == null) return;
        copiedComponents = new Component[] { comp1, comp2, comp3 };
    }

    [MenuItem("Tools/GameObject/Paste components %&P")]
    static void Paste()
    {
        if (copiedComponents == null)
        {
            Debug.LogError("Nothing is copied!");
            return;
        }

        foreach (var targetGameObject in UnityEditor.Selection.gameObjects)
        {
            if (!targetGameObject)
                continue;

            Undo.RegisterCompleteObjectUndo(targetGameObject, targetGameObject.name + ": Paste All Components"); // sadly does not record PasteComponentValues, i guess

            foreach (var copiedComponent in copiedComponents)
            {
                if (!copiedComponent)
                    continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);

                var targetComponent = targetGameObject.GetComponent(copiedComponent.GetType());

                if (targetComponent) // if gameObject already contains the component
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComponent))
                    {
                        Debug.Log("Successfully pasted: " + copiedComponent.GetType());
                    }
                    else
                    {
                        Debug.LogError("Failed to copy: " + copiedComponent.GetType());
                    }
                }
                else // if gameObject does not contain the component
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject))
                    {
                        Debug.Log("Successfully pasted: " + copiedComponent.GetType());
                    }
                    else
                    {
                        Debug.LogError("Failed to copy: " + copiedComponent.GetType());
                    }
                }
            }
        }

        copiedComponents = null; // to prevent wrong pastes in future
    }

}
#endif