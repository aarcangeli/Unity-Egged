/*
    A simple little editor extension to copy and paste all components
    Help from http://answers.unity3d.com/questions/541045/copy-all-components-from-one-character-to-another.html
    license: WTFPL (http://www.wtfpl.net/)
    author: aeroson
    advise: ChessMax
    editor: frekons
*/

using UnityEngine;
using UnityEditor;

public class ComponentsCopier
{
	static Component[] copiedComponents;

	[MenuItem("GameObject/Copy all components %#C")]
	static void Copy()
	{
		if (Selection.activeGameObject == null)
			return;

		copiedComponents = Selection.activeGameObject.GetComponents<Component>();
	}

	[MenuItem("GameObject/Paste all components %#V")]
	static void Paste()
	{
		if (copiedComponents == null)
		{
			Debug.LogError("Nothing is copied!");
			return;
		}

		foreach (var targetGameObject in Selection.gameObjects)
		{
			if (!targetGameObject)
				continue;

			// sadly does not record PasteComponentValues, i guess
			Undo.RegisterCompleteObjectUndo(targetGameObject, targetGameObject.name + ": Paste All Components");

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
	}
}
