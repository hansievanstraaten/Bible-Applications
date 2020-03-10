﻿using Bible.Models.BibleBooks;
using Bibles.Data.DataEnums;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bibles.Data
{
    public sealed class GlobalStaticData
    {
        #region Private Fields

        private readonly static object instanceLock = new object();

        private readonly static string[] keySplitValue = new string[] { "||" };

        private static GlobalStaticData instance;

        private string[] OldTestamentBookNames = new string[] { "Genesis", "Exodus","Leviticus","Numbers","Deuteronomy","Joshua"
      ,"Judges","Ruth","1 Samuel","2 Samuel","1 Kings","2 Kings","1 Chronicles","2 Chronicles","Ezra","Nehemiah","Esther","Job","Psalms"
      ,"Proverbs","Ecclesiastes","The Song of Solomon","Isaiah","Jeremiah","Lamentations","Ezekiel","Daniel","Hosea","Joel","Amos"
      ,"Obadiah","Jonah","Micah","Nahum","Habakkuk","Zephaniah","Haggai","Zechariah","Malachi" };

        private string[] NewTestamentBookNames = new string[] { "Matthew","Mark","Luke","John","Acts of the Apostles","Romans","1 Corinthians"
      ,"2 Corinthians","Galatians","Ephesians","Philippians","Colossians","1 Thessalonians","2 Thessalonians","1 Timothy","2 Timothy","Titus"
      ,"Philemon","Hebrews","James","1 Peter","2 Peter","1 John","2 John","3 John","Jude","Revelation" };

        private string[] BookChapterCount = new string[] {"01O||50||", "02O||40||", "03O||27||", "04O||36||", "05O||34||", "06O||24||", "07O||21||", "08O||4||",
      "09O||31||", "10O||24||", "11O||22||", "12O||25||", "13O||29||", "14O||36||", "15O||10||", "16O||13||", "17O||10||", "18O||42||", "19O||150||", "20O||31||",
      "21O||12||", "22O||8||", "23O||66||", "24O||52||", "25O||5||", "26O||48||", "27O||12||", "28O||14||", "29O||3||", "30O||9||", "31O||1||", "32O||4||",
      "33O||7||", "34O||3||", "35O||3||", "36O||3||", "37O||2||", "38O||14||", "39O||4||", "40N||28||", "41N||16||", "42N||24||", "43N||21||", "44N||28||",
      "45N||16||", "46N||16||", "47N||13||", "48N||6||", "49N||6||", "50N||4||", "51N||4||", "52N||5||", "53N||3||", "54N||6||", "55N||4||", "56N||3||", "57N||1||",
      "58N||13||", "59N||5||", "60N||5||", "61N||3||", "62N||5||", "63N||1||", "64N||1||", "65N||1||", "66N||22||"};

        private string[] ChapterVerseCount = new string[] { "01O||1||31", "01O||2||25", "01O||3||24", "01O||4||26", "01O||5||32", "01O||6||22", "01O||7||24",
      "01O||8||22", "01O||9||29", "01O||10||32", "01O||11||32", "01O||12||20", "01O||13||18", "01O||14||24", "01O||15||21", "01O||16||16", "01O||17||27", "01O||18||33",
      "01O||19||38", "01O||20||18", "01O||21||34", "01O||22||24", "01O||23||20", "01O||24||67", "01O||25||34", "01O||26||35", "01O||27||46", "01O||28||22", "01O||29||35",
      "01O||30||43", "01O||31||55", "01O||32||32", "01O||33||20", "01O||34||31", "01O||35||29", "01O||36||43", "01O||37||36", "01O||38||30", "01O||39||23", "01O||40||23",
      "01O||41||57", "01O||42||38", "01O||43||34", "01O||44||34", "01O||45||28", "01O||46||34", "01O||47||31", "01O||48||22", "01O||49||33", "01O||50||26", "02O||1||22",
      "02O||2||25", "02O||3||22", "02O||4||31", "02O||5||23", "02O||6||30", "02O||7||25", "02O||8||32", "02O||9||35", "02O||10||29", "02O||11||10", "02O||12||51",
      "02O||13||22", "02O||14||31", "02O||15||27", "02O||16||36", "02O||17||16", "02O||18||27", "02O||19||25", "02O||20||26", "02O||21||36", "02O||22||31", "02O||23||33",
      "02O||24||18", "02O||25||40", "02O||26||37", "02O||27||21", "02O||28||43", "02O||29||46", "02O||30||38", "02O||31||18", "02O||32||35", "02O||33||23", "02O||34||35",
      "02O||35||35", "02O||36||38", "02O||37||29", "02O||38||31", "02O||39||43", "02O||40||38", "03O||1||17", "03O||2||16", "03O||3||17", "03O||4||35", "03O||5||19",
      "03O||6||30", "03O||7||38", "03O||8||36", "03O||9||24", "03O||10||20", "03O||11||47", "03O||12||8", "03O||13||59", "03O||14||57", "03O||15||33", "03O||16||34",
      "03O||17||16", "03O||18||30", "03O||19||37", "03O||20||27", "03O||21||24", "03O||22||33", "03O||23||44", "03O||24||23", "03O||25||55", "03O||26||46", "03O||27||34",
      "04O||1||54", "04O||2||34", "04O||3||51", "04O||4||49", "04O||5||31", "04O||6||27", "04O||7||89", "04O||8||26", "04O||9||23", "04O||10||36", "04O||11||35",
      "04O||12||16", "04O||13||33", "04O||14||45", "04O||15||41", "04O||16||50", "04O||17||13", "04O||18||32", "04O||19||22", "04O||20||29", "04O||21||35", "04O||22||41",
      "04O||23||30", "04O||24||25", "04O||25||18", "04O||26||65", "04O||27||23", "04O||28||31", "04O||29||40", "04O||30||16", "04O||31||54", "04O||32||42", "04O||33||56",
      "04O||34||29", "04O||35||34", "04O||36||13", "05O||1||46", "05O||2||37", "05O||3||29", "05O||4||49", "05O||5||33", "05O||6||25", "05O||7||26", "05O||8||20",
      "05O||9||29", "05O||10||22", "05O||11||32", "05O||12||32", "05O||13||18", "05O||14||29", "05O||15||23", "05O||16||22", "05O||17||20", "05O||18||22", "05O||19||21",
      "05O||20||20", "05O||21||23", "05O||22||30", "05O||23||25", "05O||24||22", "05O||25||19", "05O||26||19", "05O||27||26", "05O||28||68", "05O||29||29", "05O||30||20",
      "05O||31||30", "05O||32||52", "05O||33||29", "05O||34||12", "06O||1||18", "06O||2||24", "06O||3||17", "06O||4||24", "06O||5||15", "06O||6||27", "06O||7||26",
      "06O||8||35", "06O||9||27", "06O||10||43", "06O||11||23", "06O||12||24", "06O||13||33", "06O||14||15", "06O||15||63", "06O||16||10", "06O||17||18", "06O||18||28",
      "06O||19||51", "06O||20||9", "06O||21||45", "06O||22||34", "06O||23||16", "06O||24||33", "07O||1||36", "07O||2||23", "07O||3||31", "07O||4||24", "07O||5||31",
      "07O||6||40", "07O||7||25", "07O||8||35", "07O||9||57", "07O||10||18", "07O||11||40", "07O||12||15", "07O||13||25", "07O||14||20", "07O||15||20", "07O||16||31",
      "07O||17||13", "07O||18||31", "07O||19||30", "07O||20||48", "07O||21||25", "08O||1||22", "08O||2||23", "08O||3||18", "08O||4||22", "09O||1||28", "09O||2||36",
      "09O||3||21", "09O||4||22", "09O||5||12", "09O||6||21", "09O||7||17", "09O||8||22", "09O||9||27", "09O||10||27", "09O||11||15", "09O||12||25", "09O||13||23",
      "09O||14||52", "09O||15||35", "09O||16||23", "09O||17||58", "09O||18||30", "09O||19||24", "09O||20||42", "09O||21||15", "09O||22||23", "09O||23||29", "09O||24||22",
      "09O||25||44", "09O||26||25", "09O||27||12", "09O||28||25", "09O||29||11", "09O||30||31", "09O||31||13", "10O||1||27", "10O||2||32", "10O||3||39", "10O||4||12",
      "10O||5||25", "10O||6||23", "10O||7||29", "10O||8||18", "10O||9||13", "10O||10||19", "10O||11||27", "10O||12||31", "10O||13||39", "10O||14||33", "10O||15||37",
      "10O||16||23", "10O||17||29", "10O||18||33", "10O||19||43", "10O||20||26", "10O||21||22", "10O||22||51", "10O||23||39", "10O||24||25", "11O||1||53", "11O||2||46",
      "11O||3||28", "11O||4||34", "11O||5||18", "11O||6||38", "11O||7||51", "11O||8||66", "11O||9||28", "11O||10||29", "11O||11||43", "11O||12||33", "11O||13||34",
      "11O||14||31", "11O||15||34", "11O||16||34", "11O||17||24", "11O||18||46", "11O||19||21", "11O||20||43", "11O||21||29", "11O||22||53", "12O||1||18", "12O||2||25",
      "12O||3||27", "12O||4||44", "12O||5||27", "12O||6||33", "12O||7||20", "12O||8||29", "12O||9||37", "12O||10||36", "12O||11||21", "12O||12||21", "12O||13||25",
      "12O||14||29", "12O||15||38", "12O||16||20", "12O||17||41", "12O||18||37", "12O||19||37", "12O||20||21", "12O||21||26", "12O||22||20", "12O||23||37", "12O||24||20",
      "12O||25||30", "13O||1||54", "13O||2||55", "13O||3||24", "13O||4||43", "13O||5||26", "13O||6||81", "13O||7||40", "13O||8||40", "13O||9||44", "13O||10||14",
      "13O||11||47", "13O||12||40", "13O||13||14", "13O||14||17", "13O||15||29", "13O||16||43", "13O||17||27", "13O||18||18", "13O||19||18", "13O||20||8", "13O||21||30",
      "13O||22||19", "13O||23||32", "13O||24||31", "13O||25||31", "13O||26||32", "13O||27||34", "13O||28||21", "13O||29||30", "14O||1||17", "14O||2||18", "14O||3||17",
      "14O||4||22", "14O||5||14", "14O||6||42", "14O||7||22", "14O||8||18", "14O||9||31", "14O||10||19", "14O||11||23", "14O||12||16", "14O||13||22", "14O||14||15",
      "14O||15||19", "14O||16||14", "14O||17||19", "14O||18||34", "14O||19||11", "14O||20||37", "14O||21||20", "14O||22||12", "14O||23||21", "14O||24||27", "14O||25||28",
      "14O||26||23", "14O||27||9", "14O||28||27", "14O||29||36", "14O||30||27", "14O||31||21", "14O||32||33", "14O||33||25", "14O||34||33", "14O||35||27", "14O||36||23",
      "15O||1||11", "15O||2||70", "15O||3||13", "15O||4||24", "15O||5||17", "15O||6||22", "15O||7||28", "15O||8||36", "15O||9||15", "15O||10||44", "16O||1||11", "16O||2||20",
      "16O||3||32", "16O||4||23", "16O||5||19", "16O||6||19", "16O||7||73", "16O||8||18", "16O||9||38", "16O||10||39", "16O||11||36", "16O||12||47", "16O||13||31",
      "17O||1||22", "17O||2||23", "17O||3||15", "17O||4||17", "17O||5||14", "17O||6||14", "17O||7||10", "17O||8||17", "17O||9||32", "17O||10||3", "18O||1||22", "18O||2||13",
      "18O||3||26", "18O||4||21", "18O||5||27", "18O||6||30", "18O||7||21", "18O||8||22", "18O||9||35", "18O||10||22", "18O||11||20", "18O||12||25", "18O||13||28",
      "18O||14||22", "18O||15||35", "18O||16||22", "18O||17||16", "18O||18||21", "18O||19||29", "18O||20||29", "18O||21||34", "18O||22||30", "18O||23||17", "18O||24||25",
      "18O||25||6", "18O||26||14", "18O||27||23", "18O||28||28", "18O||29||25", "18O||30||31", "18O||31||40", "18O||32||22", "18O||33||33", "18O||34||37", "18O||35||16",
      "18O||36||33", "18O||37||24", "18O||38||41", "18O||39||30", "18O||40||24", "18O||41||34", "18O||42||17", "19O||1||6", "19O||2||12", "19O||3||8", "19O||4||8",
      "19O||5||12", "19O||6||10", "19O||7||17", "19O||8||9", "19O||9||20", "19O||10||18", "19O||11||7", "19O||12||8", "19O||13||6", "19O||14||7", "19O||15||5", "19O||16||11",
      "19O||17||15", "19O||18||50", "19O||19||14", "19O||20||9", "19O||21||13", "19O||22||31", "19O||23||6", "19O||24||10", "19O||25||22", "19O||26||12", "19O||27||14",
      "19O||28||9", "19O||29||11", "19O||30||12", "19O||31||24", "19O||32||11", "19O||33||22", "19O||34||22", "19O||35||28", "19O||36||12", "19O||37||40", "19O||38||22",
      "19O||39||13", "19O||40||17", "19O||41||13", "19O||42||11", "19O||43||5", "19O||44||26", "19O||45||17", "19O||46||11", "19O||47||9", "19O||48||14", "19O||49||20",
      "19O||50||23", "19O||51||19", "19O||52||9", "19O||53||6", "19O||54||7", "19O||55||23", "19O||56||13", "19O||57||11", "19O||58||11", "19O||59||17", "19O||60||12",
      "19O||61||8", "19O||62||12", "19O||63||11", "19O||64||10", "19O||65||13", "19O||66||20", "19O||67||7", "19O||68||35", "19O||69||36", "19O||70||5", "19O||71||24",
      "19O||72||20", "19O||73||28", "19O||74||23", "19O||75||10", "19O||76||12", "19O||77||20", "19O||78||72", "19O||79||13", "19O||80||19", "19O||81||16", "19O||82||8",
      "19O||83||18", "19O||84||12", "19O||85||13", "19O||86||17", "19O||87||7", "19O||88||18", "19O||89||52", "19O||90||17", "19O||91||16", "19O||92||15", "19O||93||5",
      "19O||94||23", "19O||95||11", "19O||96||13", "19O||97||12", "19O||98||9", "19O||99||9", "19O||100||5", "19O||101||8", "19O||102||28", "19O||103||22", "19O||104||35",
      "19O||105||45", "19O||106||48", "19O||107||43", "19O||108||13", "19O||109||31", "19O||110||7", "19O||111||10", "19O||112||10", "19O||113||9", "19O||114||8",
      "19O||115||18", "19O||116||19", "19O||117||2", "19O||118||29", "19O||119||176", "19O||120||7", "19O||121||8", "19O||122||9", "19O||123||4", "19O||124||8",
      "19O||125||5", "19O||126||6", "19O||127||5", "19O||128||6", "19O||129||8", "19O||130||8", "19O||131||3", "19O||132||18", "19O||133||3", "19O||134||3", "19O||135||21",
      "19O||136||26", "19O||137||9", "19O||138||8", "19O||139||24", "19O||140||13", "19O||141||10", "19O||142||7", "19O||143||12", "19O||144||15", "19O||145||21",
      "19O||146||10", "19O||147||20", "19O||148||14", "19O||149||9", "19O||150||6", "20O||1||33", "20O||2||22", "20O||3||35", "20O||4||27", "20O||5||23", "20O||6||35",
      "20O||7||27", "20O||8||36", "20O||9||18", "20O||10||32", "20O||11||31", "20O||12||28", "20O||13||25", "20O||14||35", "20O||15||33", "20O||16||33", "20O||17||28",
      "20O||18||24", "20O||19||29", "20O||20||30", "20O||21||31", "20O||22||29", "20O||23||35", "20O||24||34", "20O||25||28", "20O||26||28", "20O||27||27", "20O||28||28",
      "20O||29||27", "20O||30||33", "20O||31||31", "21O||1||18", "21O||2||26", "21O||3||22", "21O||4||16", "21O||5||20", "21O||6||12", "21O||7||29", "21O||8||17", "21O||9||18",
      "21O||10||20", "21O||11||10", "21O||12||14", "22O||1||17", "22O||2||17", "22O||3||11", "22O||4||16", "22O||5||16", "22O||6||13", "22O||7||13", "22O||8||14", "23O||1||31",
      "23O||2||22", "23O||3||26", "23O||4||6", "23O||5||30", "23O||6||13", "23O||7||25", "23O||8||22", "23O||9||21", "23O||10||34", "23O||11||16", "23O||12||6", "23O||13||22",
      "23O||14||32", "23O||15||9", "23O||16||14", "23O||17||14", "23O||18||7", "23O||19||25", "23O||20||6", "23O||21||17", "23O||22||25", "23O||23||18", "23O||24||23",
      "23O||25||12", "23O||26||21", "23O||27||13", "23O||28||29", "23O||29||24", "23O||30||33", "23O||31||9", "23O||32||20", "23O||33||24", "23O||34||17", "23O||35||10",
      "23O||36||22", "23O||37||38", "23O||38||22", "23O||39||8", "23O||40||31", "23O||41||29", "23O||42||25", "23O||43||28", "23O||44||28", "23O||45||25", "23O||46||13",
      "23O||47||15", "23O||48||22", "23O||49||26", "23O||50||11", "23O||51||23", "23O||52||15", "23O||53||12", "23O||54||17", "23O||55||13", "23O||56||12", "23O||57||21",
      "23O||58||14", "23O||59||21", "23O||60||22", "23O||61||11", "23O||62||12", "23O||63||19", "23O||64||12", "23O||65||25", "23O||66||24", "24O||1||19", "24O||2||37",
      "24O||3||25", "24O||4||31", "24O||5||31", "24O||6||30", "24O||7||34", "24O||8||22", "24O||9||26", "24O||10||25", "24O||11||23", "24O||12||17", "24O||13||27",
      "24O||14||22", "24O||15||21", "24O||16||21", "24O||17||27", "24O||18||23", "24O||19||15", "24O||20||18", "24O||21||14", "24O||22||30", "24O||23||40", "24O||24||10",
      "24O||25||38", "24O||26||24", "24O||27||22", "24O||28||17", "24O||29||32", "24O||30||24", "24O||31||40", "24O||32||44", "24O||33||26", "24O||34||22", "24O||35||19",
      "24O||36||32", "24O||37||21", "24O||38||28", "24O||39||18", "24O||40||16", "24O||41||18", "24O||42||22", "24O||43||13", "24O||44||30", "24O||45||5", "24O||46||28",
      "24O||47||7", "24O||48||47", "24O||49||39", "24O||50||46", "24O||51||64", "24O||52||34", "25O||1||22", "25O||2||22", "25O||3||66", "25O||4||22", "25O||5||22",
      "26O||1||28", "26O||2||10", "26O||3||27", "26O||4||17", "26O||5||17", "26O||6||14", "26O||7||27", "26O||8||18", "26O||9||11", "26O||10||22", "26O||11||25",
      "26O||12||28", "26O||13||23", "26O||14||23", "26O||15||8", "26O||16||63", "26O||17||24", "26O||18||32", "26O||19||14", "26O||20||49", "26O||21||32", "26O||22||31",
      "26O||23||49", "26O||24||27", "26O||25||17", "26O||26||21", "26O||27||36", "26O||28||26", "26O||29||21", "26O||30||26", "26O||31||18", "26O||32||32", "26O||33||33",
      "26O||34||31", "26O||35||15", "26O||36||38", "26O||37||28", "26O||38||23", "26O||39||29", "26O||40||49", "26O||41||26", "26O||42||20", "26O||43||27", "26O||44||31",
      "26O||45||25", "26O||46||24", "26O||47||23", "26O||48||35", "27O||1||21", "27O||2||49", "27O||3||30", "27O||4||37", "27O||5||31", "27O||6||28", "27O||7||28", "27O||8||27",
      "27O||9||27", "27O||10||21", "27O||11||45", "27O||12||13", "28O||1||11", "28O||2||23", "28O||3||5", "28O||4||19", "28O||5||15", "28O||6||11", "28O||7||16", "28O||8||14",
      "28O||9||17", "28O||10||15", "28O||11||12", "28O||12||14", "28O||13||16", "28O||14||9", "29O||1||20", "29O||2||32", "29O||3||21", "30O||1||15", "30O||2||16", "30O||3||15",
      "30O||4||13", "30O||5||27", "30O||6||14", "30O||7||17", "30O||8||14", "30O||9||15", "31O||1||21", "32O||1||17", "32O||2||10", "32O||3||10", "32O||4||11", "33O||1||16",
      "33O||2||13", "33O||3||12", "33O||4||13", "33O||5||15", "33O||6||16", "33O||7||20", "34O||1||15", "34O||2||13", "34O||3||19", "35O||1||17", "35O||2||20", "35O||3||19",
      "36O||1||18", "36O||2||15", "36O||3||20", "37O||1||15", "37O||2||23", "38O||1||21", "38O||2||13", "38O||3||10", "38O||4||14", "38O||5||11", "38O||6||15", "38O||7||14",
      "38O||8||23", "38O||9||17", "38O||10||12", "38O||11||17", "38O||12||14", "38O||13||9", "38O||14||21", "39O||1||14", "39O||2||17", "39O||3||18", "39O||4||6", "40N||1||25",
      "40N||2||23", "40N||3||17", "40N||4||25", "40N||5||48", "40N||6||34", "40N||7||29", "40N||8||34", "40N||9||38", "40N||10||42", "40N||11||30", "40N||12||50", "40N||13||58",
      "40N||14||36", "40N||15||39", "40N||16||28", "40N||17||27", "40N||18||35", "40N||19||30", "40N||20||34", "40N||21||46", "40N||22||46", "40N||23||39", "40N||24||51",
      "40N||25||46", "40N||26||75", "40N||27||66", "40N||28||20", "41N||1||45", "41N||2||28", "41N||3||35", "41N||4||41", "41N||5||43", "41N||6||56", "41N||7||37", "41N||8||38",
      "41N||9||50", "41N||10||52", "41N||11||33", "41N||12||44", "41N||13||37", "41N||14||72", "41N||15||47", "41N||16||20", "42N||1||80", "42N||2||52", "42N||3||38",
      "42N||4||44", "42N||5||39", "42N||6||49", "42N||7||50", "42N||8||56", "42N||9||62", "42N||10||42", "42N||11||54", "42N||12||59", "42N||13||35", "42N||14||35",
      "42N||15||32", "42N||16||31", "42N||17||37", "42N||18||43", "42N||19||48", "42N||20||47", "42N||21||38", "42N||22||71", "42N||23||56", "42N||24||53", "43N||1||51",
      "43N||2||25", "43N||3||36", "43N||4||54", "43N||5||47", "43N||6||71", "43N||7||53", "43N||8||59", "43N||9||41", "43N||10||42", "43N||11||57", "43N||12||50", "43N||13||38",
      "43N||14||31", "43N||15||27", "43N||16||33", "43N||17||26", "43N||18||40", "43N||19||42", "43N||20||31", "43N||21||25", "44N||1||26", "44N||2||47", "44N||3||26",
      "44N||4||37", "44N||5||42", "44N||6||15", "44N||7||60", "44N||8||40", "44N||9||43", "44N||10||48", "44N||11||30", "44N||12||25", "44N||13||52", "44N||14||28",
      "44N||15||41", "44N||16||40", "44N||17||34", "44N||18||28", "44N||19||41", "44N||20||38", "44N||21||40", "44N||22||30", "44N||23||35", "44N||24||27", "44N||25||27",
      "44N||26||32", "44N||27||44", "44N||28||31", "45N||1||32", "45N||2||29", "45N||3||31", "45N||4||25", "45N||5||21", "45N||6||23", "45N||7||25", "45N||8||39", "45N||9||33",
      "45N||10||21", "45N||11||36", "45N||12||21", "45N||13||14", "45N||14||23", "45N||15||33", "45N||16||27", "46N||1||31", "46N||2||16", "46N||3||23", "46N||4||21",
      "46N||5||13", "46N||6||20", "46N||7||40", "46N||8||13", "46N||9||27", "46N||10||33", "46N||11||34", "46N||12||31", "46N||13||13", "46N||14||40", "46N||15||58",
      "46N||16||24", "47N||1||24", "47N||2||17", "47N||3||18", "47N||4||18", "47N||5||21", "47N||6||18", "47N||7||16", "47N||8||24", "47N||9||15", "47N||10||18",
      "47N||11||33", "47N||12||21", "47N||13||14", "48N||1||24", "48N||2||21", "48N||3||29", "48N||4||31", "48N||5||26", "48N||6||18", "49N||1||23", "49N||2||22",
      "49N||3||21", "49N||4||32", "49N||5||33", "49N||6||24", "50N||1||30", "50N||2||30", "50N||3||21", "50N||4||23", "51N||1||29", "51N||2||23", "51N||3||25", "51N||4||18",
      "52N||1||10", "52N||2||20", "52N||3||13", "52N||4||18", "52N||5||28", "53N||1||12", "53N||2||17", "53N||3||18", "54N||1||20", "54N||2||15", "54N||3||16", "54N||4||16",
      "54N||5||25", "54N||6||21", "55N||1||18", "55N||2||26", "55N||3||17", "55N||4||22", "56N||1||16", "56N||2||15", "56N||3||15", "57N||1||25", "58N||1||14", "58N||2||18",
      "58N||3||19", "58N||4||16", "58N||5||14", "58N||6||20", "58N||7||28", "58N||8||13", "58N||9||28", "58N||10||39", "58N||11||40", "58N||12||29", "58N||13||25", "59N||1||27",
      "59N||2||26", "59N||3||18", "59N||4||17", "59N||5||20", "60N||1||25", "60N||2||25", "60N||3||22", "60N||4||19", "60N||5||14", "61N||1||21", "61N||2||22", "61N||3||18",
      "62N||1||10", "62N||2||29", "62N||3||24", "62N||4||21", "62N||5||21", "63N||1||13", "64N||1||14", "65N||1||25", "66N||1||20", "66N||2||29", "66N||3||22", "66N||4||11",
      "66N||5||14", "66N||6||17", "66N||7||17", "66N||8||13", "66N||9||21", "66N||10||11", "66N||11||19", "66N||12||17", "66N||13||18", "66N||14||20", "66N||15||8",
      "66N||16||21", "66N||17||18", "66N||18||24", "66N||19||21", "66N||20||15", "66N||21||27", "66N||22||21"};

        private Dictionary<string, string> keyToBookIndex;

        private Dictionary<string, int> bookChapterCount;

        private Dictionary<string, int> chapterVerseCount;

        private Dictionary<string, BookModel> bookModels;

        #endregion

        public GlobalStaticData()
        {
            this.keyToBookIndex = new Dictionary<string, string>();

            this.bookModels = new Dictionary<string, BookModel>();

            this.bookChapterCount = new Dictionary<string, int>();

            this.chapterVerseCount = new Dictionary<string, int>();

            this.InitializeDictionaryData();
        }

        public static GlobalStaticData Intance
        {
            get
            {
                if (GlobalStaticData.instance == null)
                {
                    lock (GlobalStaticData.instanceLock)
                    {
                        if (GlobalStaticData.instance == null)
                        {
                            GlobalStaticData.instance = new GlobalStaticData();
                        }
                    }
                }

                return GlobalStaticData.instance;
            }
        }

        public string GetBookName(string key)
        {
            return keyToBookIndex[this.GetBookKey(key)];
        }

        public List<BookModel> GetTestament(TestamentEnum testament)
        {
            return this.bookModels
                    .Where(b => b.Key.EndsWith(testament.ToString()))
                    .Select(s => s.Value)
                    .ToList();
        }

        #region PRIVATE GET-METHODS

        private string GetBookKey(string key)
        {
            return key.Substring(0, 3);
        }

        private string GetChapterKey(string key)
        {
            string[] keyItems = key.Split(keySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return $"{keyItems[0]}||{keyItems[1]}";
        }

        private int GetChapterCount(string key)
        {
            string[] chapterSplit = key.Split(keySplitValue, StringSplitOptions.RemoveEmptyEntries);

            if (chapterSplit.Length < 2)
            {
                return 0;
            }

            return chapterSplit[1].ToInt32();
        }

        private int GetVerseCount(string key)
        {
            string[] chapterSplit = key.Split(keySplitValue, StringSplitOptions.RemoveEmptyEntries);

            if (chapterSplit.Length < 3)
            {
                return 0;
            }

            return chapterSplit[2].ToInt32();
        }

        #endregion

        #region KEY BUILD METHODS
        private string BuildBookKey(int index, TestamentEnum testament)
        {
            return $"{index.ParseToString().PadLeft(2, '0')}{testament}";
        }

        private string BuildChapterKey(string bookKey, int chapterNumber)
        {
            return $"{bookKey}||{chapterNumber}";
        }

        private string BuildVerseKey(string chapterKey, int verseNumber)
        {
            return $"{chapterKey}||{verseNumber}";
        }

        #endregion

        #region LOAD 

        private void InitializeDictionaryData()
        {
            foreach (string chapter in this.BookChapterCount)
            {
                this.bookChapterCount.Add(this.GetBookKey(chapter), this.GetChapterCount(chapter));
            }

            foreach (string verse in this.ChapterVerseCount)
            {
                this.chapterVerseCount.Add(this.GetChapterKey(verse), this.GetVerseCount(verse));
            }

            int xOffset = 1;

            for (int x = 0; x < this.OldTestamentBookNames.Length; ++x)
            {
                string bookKey = this.BuildBookKey((x + xOffset), TestamentEnum.O);

                this.keyToBookIndex.Add(bookKey, this.OldTestamentBookNames[x]);

                BookModel book = new BookModel
                {
                    BookKey = bookKey,
                    BookName = this.keyToBookIndex[bookKey]
                };

                this.LoadBookChapters(book);

                this.bookModels.Add(bookKey, book);
            }

            xOffset = 40;

            for (int x = 0; x < this.NewTestamentBookNames.Length; ++x)
            {
                string bookKey = this.BuildBookKey((x + xOffset), TestamentEnum.N);

                keyToBookIndex.Add(this.BuildBookKey((x + xOffset), TestamentEnum.N), this.NewTestamentBookNames[x]);

                BookModel book = new BookModel
                {
                    BookKey = bookKey,
                    BookName = this.keyToBookIndex[bookKey]
                };

                this.LoadBookChapters(book);

                this.bookModels.Add(bookKey, book);
            }
        }

        private void LoadBookChapters(BookModel book)
        {
            int chapterCount = this.bookChapterCount[book.BookKey];

            for (int x = 1; x <= chapterCount; ++x)
            {
                ChapterModel chapter = new ChapterModel
                {
                    ChapterNumber = x,
                    ChapterKey = this.BuildChapterKey(book.BookKey, x)
                };

                this.LoadChapterVerses(chapter);

                book.Chapters.Add(x, chapter);
            }
        }

        private void LoadChapterVerses(ChapterModel chapter)
        {
            int verseCount = this.chapterVerseCount[chapter.ChapterKey];

            for(int x = 1; x <= verseCount; ++x)
            {
                chapter.Verses.Add(x,
                                   new  VerseModel
                                    {
                                        VerseKey = this.BuildVerseKey(chapter.ChapterKey, x),
                                        VerseNumber = x
                                    });
            }
        }

        #endregion
    }
}
