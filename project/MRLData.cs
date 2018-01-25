﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using BSLib;
using MysteriesRL.Items;
using MysteriesRL.Maps;

namespace MysteriesRL
{
    public struct BodypartRec
    {
        public readonly String Name;
        public readonly float DmgMultiply;
        public readonly EquipmentType Equipment;

        public BodypartRec(String name, float dmgMultiply, EquipmentType equipment)
        {
            Name = name;
            DmgMultiply = dmgMultiply;
            Equipment = equipment;
        }
    }

    public struct ItemTypeRec
    {
        public readonly int Value;
        public readonly char Sym;
        public readonly Color SymColor;
        public readonly float Weight;
        public readonly bool IsStacked;
        public readonly EquipmentType Equipment;
        public readonly float PocketCapacity;

        public ItemTypeRec(int value, char sym, Color symColor, float weight,
            bool isStacked, EquipmentType equipment, float pocketCapacity)
        {
            Value = value;
            Sym = sym;
            SymColor = symColor;
            Weight = weight;
            IsStacked = isStacked;
            Equipment = equipment;
            PocketCapacity = pocketCapacity;
        }
    }

    public struct HouseStatusRec
    {
        public readonly String Name;

        public HouseStatusRec(String name)
        {
            Name = name;
        }
    }

    public struct TileRec
    {
        public readonly int Value;
        public readonly char Sym;
        public readonly Color SymColor;
        public readonly Color BackColor;
        public readonly bool MiniMap;
        public readonly TileFlags Flags;
        public readonly float Cost;

        public TileRec(int value, char sym, Color symColor, Color backColor, bool miniMap, TileFlags flags, float cost)
        {
            Value = value;
            Sym = sym;
            SymColor = symColor;
            BackColor = backColor;
            MiniMap = miniMap;
            Flags = flags;
            Cost = cost;
        }
    }

    public struct ProsperityRec
    {
        public readonly int Value;
        public readonly int MinRoomSize;
        public readonly int MaxRoomSize;
        public readonly int MinHouseSize;
        public readonly int MaxHouseSize;
        public readonly int BuildSpaces;

        public ProsperityRec(int value, int minRoomSize, int maxRoomSize, int minHouseSize, int maxHouseSize, int buildSpaces)
        {
            Value = value;
            MinRoomSize = minRoomSize;
            MaxRoomSize = maxRoomSize;
            MinHouseSize = minHouseSize;
            MaxHouseSize = maxHouseSize;
            BuildSpaces = buildSpaces;
        }
    }

    public static class MRLData
    {
        public const string MRL_NAME = "The MysteriesRL";
        public const string MRL_VER = "v0.5.0.0";
        public const string MRL_DEV_TIME = "2015";
        public static readonly string MRL_COPYRIGHT = "Copyright (c) " + MRL_DEV_TIME + " by Serg V. Zhdanovskih";

        public const string MRL_DEFLANG = "en";

        // Game viewport
        public static readonly ExtRect GV_BOUNDS = ExtRect.Create(1, 1, 101, 69);
        // 80 - (4 + 7) = 80 - 11 = 69
        public const int GV_XRad = 50;
        public const int GV_YRad = 34;

        public const bool DEBUG_DISTRICTS = false;
        public const bool DEBUG_CITYGEN = false;
        public const bool DEBUG_BODY = true;

        // Without dungeons 
        public const bool DEBUG_WD = true;


        public static readonly BodypartRec[] Bodyparts = new BodypartRec[8];
        public static readonly ItemTypeRec[] ItemTypes = new ItemTypeRec[6];
        public static readonly HouseStatusRec[] HouseStatuses = new HouseStatusRec[5];
        public static readonly ProsperityRec[] Prosperities = new ProsperityRec[3];
        public static readonly Dictionary<int, TileRec> Tiles = new Dictionary<int, TileRec>();

        private static void AddTile(TileRec rec)
        {
            Tiles.Add(rec.Value, rec);
        }

        public static TileRec GetTileRec(int tileId)
        {
            TileRec result;
            if (Tiles.TryGetValue(tileId, out result)) return result;
            //throw new Exception("tileId is not found");
            return Tiles[1];
        }

        static MRLData()
        {
            Bodyparts[0] = new BodypartRec("head", 3, EquipmentType.ET_HEAD);
            Bodyparts[1] = new BodypartRec("torso", 1, EquipmentType.ET_TORSO);
            Bodyparts[2] = new BodypartRec("left arm", 1, EquipmentType.ET_LHAND);
            Bodyparts[3] = new BodypartRec("right arm", 1, EquipmentType.ET_RHAND);
            Bodyparts[4] = new BodypartRec("left leg", 1, EquipmentType.ET_NONE);
            Bodyparts[5] = new BodypartRec("right leg", 1, EquipmentType.ET_NONE);
            Bodyparts[6] = new BodypartRec("left eye", 0.2f, EquipmentType.ET_NONE);
            Bodyparts[7] = new BodypartRec("right eye", 0.2f, EquipmentType.ET_NONE);

            ItemTypes[0] = new ItemTypeRec(1, '$', Color.Yellow, 0.01f, true, EquipmentType.ET_POCKET, 0.00f); // IT_COIN
            ItemTypes[1] = new ItemTypeRec(2, 'x', Color.White, 1f, false, EquipmentType.ET_SHIRT, 10.0f); // IT_SHIRT
            ItemTypes[2] = new ItemTypeRec(3, 'x', Color.White, 2f, false, EquipmentType.ET_PANTS, 10.0f); // IT_PANTS
            ItemTypes[3] = new ItemTypeRec(4, 'x', Color.White, 2f, false, EquipmentType.ET_PANTS, 10.0f); // IT_SKIRT
            ItemTypes[4] = new ItemTypeRec(5, 'x', Color.White, 2f, false, EquipmentType.ET_BOOTS, 0.00f); // IT_BOOTS
            ItemTypes[5] = new ItemTypeRec(6, '(', Color.Red, 2f, false, EquipmentType.ET_RHAND, 0.00f); // IT_KNIFE

            HouseStatuses[0] = new HouseStatusRec(""); // hsNone
            HouseStatuses[1] = new HouseStatusRec("shack"); // hsShack: лачуга, хижина, хибара, хибарка
            HouseStatuses[2] = new HouseStatusRec("house"); // hsHouse: обычный дом
            HouseStatuses[3] = new HouseStatusRec("detached house"); // hsDetachedHouse: особняк, отдельный дом
            HouseStatuses[4] = new HouseStatusRec("mansion"); // hsMansion: большой особняк, большой дом; дворец

            Prosperities[0] = new ProsperityRec(0, 5, 10, 10, 15, 1); // pPoor
            Prosperities[1] = new ProsperityRec(1, 6, 12, 18, 20, 4); // pGood
            Prosperities[2] = new ProsperityRec(2, 10, 20, 40, 70, 10); // pLux


            AddTile(new TileRec(1, '.', Color.DarkGray, Color.Black, false, new TileFlags(), 1.25f)); // tid_Ground
            AddTile(new TileRec(2, '.', Color.Green, Color.DarkGreen, true, new TileFlags(), 1.5f)); // tid_Grass
            AddTile(new TileRec(3, (char)176, Color.LightGray, Color.Black, true, new TileFlags(), 1.0f)); // tid_Road
            AddTile(new TileRec(4, 'T', Color.Green, Color.DarkGreen, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Tree
            AddTile(new TileRec(5, (char)177, Color.DarkGray, Color.Black, true, new TileFlags(), 1.0f)); // tid_Square
            AddTile(new TileRec(6, (char)247, Color.Cyan, Color.Blue, true, new TileFlags(), 1000.0f)); // tid_Water
            AddTile(new TileRec(6 | (1 << 8), (char)247, Color.DarkBlue, Color.Blue, true, new TileFlags(), 1000.0f)); // tid_Water2
            AddTile(new TileRec(6 | (2 << 8), (char)247, Color.DarkCyan, Color.Blue, true, new TileFlags(), 1000.0f)); // tid_Water3
            AddTile(new TileRec(7, (char)177, Color.DarkGray, Color.Black, false, new TileFlags(TileFlags.tfForeground), 30.0f)); // tid_Grave

            AddTile(new TileRec(20, '+', Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_HouseWall
            AddTile(new TileRec(21, '/', Color.Magenta, Color.Black, false, new TileFlags(TileFlags.tfBlockLOS, TileFlags.tfForeground), 20.0f)); // tid_HouseDoor
            AddTile(new TileRec(22, '#', Color.White, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_HouseBorder
            AddTile(new TileRec(23, '%', Color.Cyan, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_HouseWindow
            AddTile(new TileRec(24, '.', Color.LightGray, Color.Black, false, new TileFlags(), 1.0f)); // tid_HouseFloor

            AddTile(new TileRec(31, (char)196, Color.Cyan, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_HouseWindowH
            AddTile(new TileRec(32, (char)179, Color.Cyan, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_HouseWindowV
            AddTile(new TileRec(33, '%', Color.Cyan, Color.Black, false, new TileFlags(TileFlags.tfForeground), 1.0f)); // tid_HouseWindowB

            AddTile(new TileRec(34, '/', Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfForeground), 20.0f)); // tid_HouseDoorHO
            AddTile(new TileRec(35, (char)196, Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_HouseDoorHC
            AddTile(new TileRec(36, '\\', Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfForeground), 20.0f)); // tid_HouseDoorVO
            AddTile(new TileRec(37, (char)179, Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_HouseDoorVC 

            AddTile(new TileRec(38, '>', Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfForeground), 20.0f)); // tid_StairsA
            AddTile(new TileRec(39, '<', Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfForeground), 20.0f)); // tid_StairsD

            //AddTile(new TileRec(50, 'r', Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_RoomWall
            AddTile(new TileRec(50 | (0 << 8), (char)218, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SLT
            AddTile(new TileRec(50 | (1 << 8), (char)191, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SRT
            AddTile(new TileRec(50 | (2 << 8), (char)192, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SLB
            AddTile(new TileRec(50 | (3 << 8), (char)217, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SRB
            AddTile(new TileRec(50 | (4 << 8), (char)196, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SH
            AddTile(new TileRec(50 | (5 << 8), (char)179, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SV
            AddTile(new TileRec(50 | (6 << 8), (char)193, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SBI
            AddTile(new TileRec(50 | (7 << 8), (char)194, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_STI
            AddTile(new TileRec(50 | (8 << 8), (char)195, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SLI
            AddTile(new TileRec(50 | (9 << 8), (char)180, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SRI
            AddTile(new TileRec(50 | (10 << 8), (char)197, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_SC

            //AddTile(new TileRec(70, 'b', Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_BlockWall
            AddTile(new TileRec(70 | ( 0 << 8), (char)201, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DLT
            AddTile(new TileRec(70 | ( 1 << 8), (char)187, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DRT
            AddTile(new TileRec(70 | ( 2 << 8), (char)200, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DLB
            AddTile(new TileRec(70 | ( 3 << 8), (char)188, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DRB
            AddTile(new TileRec(70 | ( 4 << 8), (char)205, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DH
            AddTile(new TileRec(70 | ( 5 << 8), (char)186, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DV
            AddTile(new TileRec(70 | ( 6 << 8), (char)202, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DBI
            AddTile(new TileRec(70 | ( 7 << 8), (char)203, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DTI
            AddTile(new TileRec(70 | ( 8 << 8), (char)204, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DLI
            AddTile(new TileRec(70 | ( 9 << 8), (char)185, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DRI
            AddTile(new TileRec(70 | (10 << 8), (char)206, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DC
            AddTile(new TileRec(70 | (11 << 8), (char)207, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DSBI
            AddTile(new TileRec(70 | (12 << 8), (char)209, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DSTI
            AddTile(new TileRec(70 | (13 << 8), (char)199, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DSLI
            AddTile(new TileRec(70 | (14 << 8), (char)182, Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f)); // tid_Wall_DSRI

            AddTile(new TileRec(101, '!', Color.Red, Color.Black, false, new TileFlags(TileFlags.tfForeground), 1.0f)); // tid_PathStart
            AddTile(new TileRec(102, '!', Color.White, Color.Black, false, new TileFlags(TileFlags.tfForeground), 1.0f)); // tid_PathFinish
            AddTile(new TileRec(103, '*', Color.Cyan, Color.Black, false, new TileFlags(TileFlags.tfForeground), 1.0f)); // tid_Path

            AddTile(new TileRec(150, 'S', Color.Red, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_Stove
            AddTile(new TileRec(151, 'B', Color.LightGray, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_Bed

            AddTile(new TileRec(171, '.', Color.DarkGray, Color.Black, false, new TileFlags(), 1.0f)); // tid_DungeonFloor
            AddTile(new TileRec(172, 'X', Color.DarkGray, Color.Black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f)); // tid_DungeonWall

            AddTile(new TileRec(200, 'D', Color.Red, Color.Black, true, new TileFlags(), 1.0f)); // tid_DebugDistrictP
            AddTile(new TileRec(201, 'D', Color.Yellow, Color.Black, true, new TileFlags(), 1.0f)); // tid_DebugDistrictG
            AddTile(new TileRec(202, 'D', Color.Green, Color.Black, true, new TileFlags(), 1.0f)); // tid_DebugDistrictL
        }
    }
}
