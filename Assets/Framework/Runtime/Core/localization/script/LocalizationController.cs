
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;

public class LocalizationController : SingletonMonoBehaviour<LocalizationController>
{
    #region data members

    public TMP_FontAsset fontAsset { get; private set; }
    public ReactiveProperty<SystemLanguage> languageRx { get; private set; }
    private readonly Dictionary<string, string> dicLocalizationText = new();
    private readonly Dictionary<SystemLanguage, Dictionary<string, string>> dicAllLocalizationText = new();
    private readonly Dictionary<Color, Material> dicFontMaterial = new();

    #endregion

    #region load data

    public async UniTask LoadLocalizationData(ReactiveProperty<SystemLanguage> languageRx)
    {
        this.languageRx = languageRx;
        await LoadLocalizationData(languageRx.Value);
    }
    
    public async UniTask LoadLocalizationData(SystemLanguage language)
    {
        dicFontMaterial.Clear();

        //load font asset
        var languageInfo = new LanguageInfoConfig();
        var setType = languageInfo.GetLanguageInfoItem(language).groupName;
        fontAsset = await AssetManager.instance.LoadTmpFont("localization/font", setType.ToString());

        //load texts
        dicLocalizationText.Clear();

        var keys = await ReadDataFile("_key", AssetManager.instance);
        var texts = await ReadDataFile(language.ToString(), AssetManager.instance);
        for (var i = 0; i < keys.Count; i++)
        {
            dicLocalizationText.Add(keys[i], texts[i]);
        }
    }

    public async UniTask LoadAllLocalizationData()
    {
        var assetManager = new AssetManager();
        var keys = await ReadDataFile("_key", assetManager);
        var lLangs = GetSupportedLanguages();
        foreach (var lang in lLangs)
        {
            var dic = new Dictionary<string, string>();
            var texts = await ReadDataFile(lang.ToString(), assetManager);
            for (var i = 0; i < keys.Count; i++)
            {
                dic.Add(keys[i], texts[i]);
            }
            dicAllLocalizationText.Add(lang, dic);
        }
    }

    private async UniTask<List<string>> ReadDataFile(string filename, AssetManager assetManager)
    {
        var separator = '\0';
        var txt = (await assetManager.LoadText("localization/text", filename)).text;
        txt = txt.TrimEnd(separator);
        return new List<string>(txt.Split(separator));
    }

    #endregion

    #region public utils

    public void SetupLocalizedText(LocalizedText localizedText)
    {
        if (string.IsNullOrEmpty(localizedText.key))
        {
            return;
        }
        var localizedTxt = GetLocalizationText(localizedText.key);
        if (localizedText.parameters != null && localizedText.parameters.Length > 0)
        {
            try
            {
                localizedTxt = string.Format(localizedTxt, localizedText.parameters);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(
                    $"[localize] format string fail, str={localizedTxt} nParams={localizedText.parameters.Length}");
            }
        }

        localizedText.SetText(localizedTxt);
    }

    public void SetUnderlayColorForTMP(TMP_Text text, Color color)
    {
        //by default, all text use one single material fontSharedMaterial => 1 draw call
        //when text get fontMaterial, it will clone fontSharedMaterial => 1 more draw call

        if (dicFontMaterial.ContainsKey(color))
        {
            text.fontMaterial = dicFontMaterial[color];
            text.fontSharedMaterial = dicFontMaterial[color];
        }
        else
        {
            text.fontMaterial.SetColor("_UnderlayColor", color);
            dicFontMaterial.Add(color, text.fontMaterial);
        }
    }

    public string LocalizedTextParameterToString(LocalizedTextParameter param)
    {
        var localizedTxt = GetLocalizationText(param.key);
        if (param.parameters != null && param.parameters.Length > 0)
        {
            try
            {
                localizedTxt = string.Format(localizedTxt, param.parameters);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(
                    $"[localize parameter] format string fail, str={localizedTxt} nParams={param.parameters.Length}");
            }
        }

        return localizedTxt;
    }

    public Dictionary<SystemLanguage, string> GetLocalizationTextAllLangs(string key)
    {
        var dic = new Dictionary<SystemLanguage, string>();
        foreach (var i in dicAllLocalizationText)
        {
            dic.Add(i.Key, i.Value[key]);
        }

        return dic;
    }

    public static List<SystemLanguage> GetSupportedLanguages()
    {
        var langInfo = new LanguageInfoConfig();
        return new List<SystemLanguage>(langInfo.languageInfoItems.Keys);
    }

    #endregion

    #region private utils

    //must be private, only set key+parameter for text object,
    //not set whole text for text object
    private string GetLocalizationText(string key)
    {
        if (dicLocalizationText.ContainsKey(key))
        {
            return dicLocalizationText[key];
        }
        else
        {
            throw new Exception($"[Localization] there's no key {key}");
        }
    }
    
    #endregion
}