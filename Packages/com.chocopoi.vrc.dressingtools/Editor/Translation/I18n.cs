﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Chocopoi.DressingTools.Translation
{
    public class I18n
    {
        private static readonly string DEFAULT_LOCALE = "en-uk";

        private static I18n instance = null;

        public static I18n GetInstance()
        {
            return instance ?? (instance = new I18n());
        }

        private Dictionary<string, JObject> translations = null;

        private string selectedLocale = null;

        private I18n()
        {
            LoadTranslations(new string[] { "en-uk", "zh-tw", "jp", "kor", "fr" });
        }

        public void LoadTranslations(string[] locales)
        {
            translations = new Dictionary<string, JObject>(locales.Length);
            foreach (string locale in locales)
            {
                try
                {
                    StreamReader reader = new StreamReader("Packages/com.chocopoi.vrc.dressingtools/Translations/" + locale + ".json");
                    string json = reader.ReadToEnd();
                    reader.Close();
                    translations.Add(locale, JObject.Parse(json));
                }
                catch (IOException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public void SetLocale(string locale)
        {
            selectedLocale = locale;
        }

        public string _(string key, params object[] args)
        {
            return Translate(key, args);
        }

        public string Translate(string key, params object[] args)
        {
            return Translate(key, null, args);
        }

        public string Translate(string key, string fallback = null, params object[] args)
        {
            string value;

            if ((value = TranslateByLocale(selectedLocale, key, args)) != null)
            {
                return value;
            }

            if ((value = TranslateByLocale(DEFAULT_LOCALE, key, args)) != null)
            {
                return value;
            }

            return fallback ?? key;
        }

        public string TranslateByLocale(string locale, string key, params object[] args)
        {
            if (locale != null && translations.ContainsKey(locale))
            {
                translations.TryGetValue(locale, out JObject t);

                string value = t?.Value<string>(key);

                if (value != null)
                {
                    return string.Format(value, args);
                }
            }

            return null;
        }
    }
}

