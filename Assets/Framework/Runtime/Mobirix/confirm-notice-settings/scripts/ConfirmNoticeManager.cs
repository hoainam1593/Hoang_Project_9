
using UnityEngine;

public static class ConfirmNoticeManager
{
    public static void OpenConfirmNotice(bool isAgree, bool isNoticeAtNight)
    {
        var namePrefab = Screen.width > Screen.height ? "confirm-notice-horizontal" : "confirm-notice-vertical";
        var prefab = Resources.Load<GameObject>(namePrefab);
        var obj = Object.Instantiate(prefab).GetComponent<ConfirmNoticeView>();

        obj.Init(isAgree, isNoticeAtNight);
    }
}
