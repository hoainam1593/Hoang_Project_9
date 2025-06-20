using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateRecycleViewUtils
{
	public static ScrollRect CreateScrollView(GameObject parent, bool isVertical)
	{
		var scrollRect = CreateScrollRectGameObject(parent, isVertical);
		var viewport = CreateViewportGameObject(scrollRect.gameObject);
		var content = CreateContentGameObject(viewport.gameObject, isVertical);

		scrollRect.viewport = viewport;
		scrollRect.content = content;
		Selection.activeObject = scrollRect.gameObject;

		return scrollRect;
	}

	private static ScrollRect CreateScrollRectGameObject(GameObject parent, bool isVertical)
	{
		//create game object
		var scroll = StaticUtilsEditor.CreateGameObject<ScrollRect>(parent, "recycle-scrollview");

		//add component
		scroll.gameObject.AddComponent<Image>();

		//configure scroll
		scroll.horizontal = !isVertical;
		scroll.vertical = isVertical;

		//return
		return scroll;
	}

	private static RectTransform CreateViewportGameObject(GameObject scrollRectGameObject)
	{
		var rect = StaticUtilsEditor.CreateGameObject<RectTransform>(scrollRectGameObject, "viewport");
		rect.gameObject.AddComponent<RectMask2D>();
		rect.anchorMin = new Vector2(0, 0);
		rect.anchorMax = new Vector2(1, 1);
		rect.offsetMax = new Vector2(0, 0);
		rect.offsetMin = new Vector2(0, 0);
		return rect;
	}

	private static RectTransform CreateContentGameObject(GameObject viewportGameObject, bool isVertical)
	{
		var rect = StaticUtilsEditor.CreateGameObject<RectTransform>(viewportGameObject, "content");
		if (isVertical)
		{
			rect.anchorMin = new Vector2(0, 1);
			rect.anchorMax = new Vector2(1, 1);
			rect.pivot = new Vector2(0.5f, 1);
			rect.offsetMax = new Vector2(0, 0);
			rect.offsetMin = new Vector2(0, -100);
		}
		else
		{
			rect.anchorMin = new Vector2(0, 0);
			rect.anchorMax = new Vector2(0, 1);
			rect.pivot = new Vector2(0, 0.5f);
			rect.offsetMax = new Vector2(100, 0);
			rect.offsetMin = new Vector2(0, 0);
		}
		return rect;
	}
}