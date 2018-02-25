/*
 *  "PrimevalRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Reflection;
using ZRLib.Core;

namespace PrimevalRL.Game
{
    public static class RS
    {
        public const short
            Rs_Reserved = 0,
            Rs_NewGame = 1,
            Rs_Quit = 2,
            Rs_PlayerChoices = 3,
            Rs_PressNumberOfChoice = 4,
            Rs_Location = 5,
            Rs_LayerGeneration = 6,
            Rs_CityGeneration = 7,
            Rs_DistrictsGeneration = 8,
            Rs_StreetsGeneration = 9,
            Rs_BuildingsPopulate = 10,
            Rs_DistrictsBuildingsGeneration = 11,
            Rs_CellarsGeneration = 12,
            Rs_DungeonsGeneration = 13,
            Rs_HeIsNYearsOld = 14,
            Rs_SheIsNYearsOld = 15,
            Rs_HeHasXHairAndYEyes = 16,
            Rs_SheHasXHairAndYEyes = 17,
            Rs_5 = 18,
            Rs_6 = 19,
            Rs_7 = 20,
            Rs_8 = 21,
            Rs_First = 1,
            Rs_Last = 21;
    }

    public sealed class Locale : BaseLocale
    {
        static Locale()
        {
            InitList(RS.Rs_Last + 1);
        }

        public Locale()
        {
            LoadLangTexts();
        }

        private void LoadLangTexts()
        {
            try {
                Assembly assembly = typeof(Locale).Assembly;
                using (Stream stm = assembly.GetManifestResourceStream("resources." + MRLData.MRL_DEFLANG + "_texts.xml")) {
                    base.LoadLangTexts(stm);
                }
            } catch (Exception ex) {
                Logger.Write("Locale.loadLangTexts(): " + ex.Message);
            }
        }
    }
}
