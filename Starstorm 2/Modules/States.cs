﻿using System.Collections.Generic;
using System;
using EntityStates.SS2UStates.Common.Emotes;
using EntityStates.SS2UStates.Common;

namespace Starstorm2Unofficial.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void Initialize()
        {
            AddState(typeof(BaseEmote));
            AddState(typeof(RestEmote));
            AddState(typeof(TauntEmote));

            AddState(typeof(NemesisSpawnState));

            AddState(typeof(BaseCustomMainState));
            AddState(typeof(BaseCustomSkillState));
            AddState(typeof(BaseMeleeAttack));
        }

        internal static void AddState(Type t)
        {
            entityStates.Add(t);
        }
    }
}