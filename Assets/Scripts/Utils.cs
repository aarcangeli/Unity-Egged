using UnityEngine;

public class Utils
{
	public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
	{
		return (layerMask.value & (1 << obj.layer)) > 0;
	}
}
