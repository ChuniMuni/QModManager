﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QModManager.API
{
    public partial class QModAPI : IQModAPI
    {
        public static IQModAPI Main { get; } = new QModAPI();
        private QModAPI() { }

        internal static List<Assembly> ErroredMods = new List<Assembly>();

        public static void MarkAsErrored(Assembly modAssembly = null)
            => Main.MarkAsErrored(modAssembly);

        public static ReadOnlyCollection<IQMod> GetAllMods(bool includeUnloaded = false, bool includeErrored = false)
            => Main.GetAllMods(includeUnloaded, includeErrored);

        public static IQMod GetMyMod()
            => Main.GetMyMod();
        public static IQMod GetMod(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
            => Main.GetMod(modAssembly, includeUnloaded, includeErrored);
        public static IQMod GetMod(string id, bool includeUnloaded = false, bool includeErrored = false)
            => Main.GetMod(id, includeUnloaded, includeErrored);

        public static bool ModPresent(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
            => Main.ModPresent(modAssembly, includeUnloaded, includeErrored);
        public static bool ModPresent(string id, bool includeUnloaded = false, bool includeErrored = false)
            => Main.ModPresent(id, includeUnloaded, includeErrored);

        #region Non-static

#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments

        void IQModAPI.MarkAsErrored(Assembly modAssembly = null)
        {
            if (ErroredMods.Contains(modAssembly)) return;

            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();
            ErroredMods.Add(modAssembly);
        }

        ReadOnlyCollection<IQMod> IQModAPI.GetAllMods(bool includeUnloaded = false, bool includeErrored = false)
        {
            if (includeErrored)
                return Patcher.foundMods.Select(qmod => (IQMod)qmod).ToList().AsReadOnly();
            else if (includeUnloaded)
                return Patcher.sortedMods.Select(qmod => (IQMod)qmod).ToList().AsReadOnly();
            else
                return Patcher.loadedMods.Select(qmod => (IQMod)qmod).ToList().AsReadOnly();
        }

        IQMod IQModAPI.GetMyMod()
            => GetMod(Assembly.GetCallingAssembly(), true, true);
        IQMod IQModAPI.GetMod(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
        {
            if (modAssembly == null) return null;

            foreach (QMod mod in GetAllMods(includeUnloaded, includeErrored))
                if (mod.LoadedAssembly == modAssembly) return mod;
            return null;
        }
        IQMod IQModAPI.GetMod(string id, bool includeUnloaded = false, bool includeErrored = false)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (QMod mod in GetAllMods(includeUnloaded, includeErrored))
                if (mod.Id == Regex.Replace(id, Patcher.IDRegex, "", RegexOptions.IgnoreCase)) return mod;
            return null;
        }

        bool IQModAPI.ModPresent(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
            => GetMod(modAssembly, includeUnloaded, includeErrored) != null;
        bool IQModAPI.ModPresent(string id, bool includeUnloaded = false, bool includeErrored = false)
            => GetMod(id, includeUnloaded, includeErrored) != null;

#pragma warning restore CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
        
        #endregion
    }

    public partial interface IQModAPI
    {
        void MarkAsErrored(Assembly modAssembly = null);

        ReadOnlyCollection<IQMod> GetAllMods(bool includeUnloaded = false, bool includeErrored = false);

        IQMod GetMyMod();
        IQMod GetMod(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false);
        IQMod GetMod(string id, bool includeUnloaded = false, bool includeErrored = false);

        bool ModPresent(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false);
        bool ModPresent(string id, bool includeUnloaded = false, bool includeErrored = false);
    }
}
