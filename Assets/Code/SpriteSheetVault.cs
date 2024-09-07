using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class SpriteSheetVault : MonoBehaviour
    {
        private static Dictionary<SpriteSheetId, SpriteSheetAsset.Sheet> s_idToSheet;
        private static Dictionary<string, SpriteSheetId> s_nameToId;

        public static bool IsReady { get; private set; }

        public static bool TryGetSheet(SpriteSheetId id, out SpriteSheetAsset.Sheet sheet)
            => s_idToSheet.TryGetValue(id, out sheet);

        public static bool TryGetSheet(string name, out SpriteSheetAsset.Sheet sheet)
        {
            if (s_nameToId.TryGetValue(name, out var id))
            {
                return s_idToSheet.TryGetValue(id, out sheet);
            }

            sheet = default;
            return false;
        }

        public static bool TryGetId(string name, out SpriteSheetId id)
            => s_nameToId.TryGetValue(name, out id);

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            IsReady = false;
            s_idToSheet = null;
            s_nameToId = null;
        }
#endif

        [SerializeField] private SpriteSheetAsset[] _assets;

        private void Awake()
        {
            var assets = _assets.AsSpan();
            s_idToSheet = new();
            s_nameToId = new();

            foreach (var asset in assets)
            {
                if (asset == false) continue;

                var assetId = asset.Id;
                var assetName = asset.name;
                var sheets = asset.Sheets.Span;

                foreach (var sheet in sheets)
                {
                    var sheetId = sheet.Id;
                    var sheetName = sheet.Name;

                    if (sheet.Sprites.Length < 1) continue;

                    var id = new SpriteSheetId(assetId, sheetId);

                    if (s_idToSheet.ContainsKey(id))
                    {
                        ErrorIdAlreadyRegistered(id, sheetName, assetName);
                        continue;
                    }

                    var name = $"{assetName}-{sheetName}";

                    if (s_nameToId.ContainsKey(name))
                    {
                        ErrorNameAlreadyRegistered(name, id, sheetName, assetName);
                        continue;
                    }

                    s_idToSheet[id] = sheet;
                    s_nameToId[name] = id;
                }
            }

            IsReady = true;
        }

        private static void ErrorIdAlreadyRegistered(SpriteSheetId id, string sheetName, string assetName)
        {
            Debug.LogError($"Cannot register id '{id}' for sheet '{sheetName}' in asset '{assetName}' " +
                $"because the same id is already used for another sheet."
            );
        }

        private static void ErrorNameAlreadyRegistered(string name, SpriteSheetId id, string sheetName, string assetName)
        {
            Debug.LogError($"Cannot register name '{name}' for id '{id}' for sheet '{sheetName}' in asset '{assetName}' " +
                $"because the same name is already used for another sheet."
            );
        }
    }
}
