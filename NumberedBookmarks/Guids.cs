// Guids.cs
// MUST match guids.h
using System;

namespace NumberedBookmarks
{
    static class GuidList
    {
        public const string guidNumberedBookmarksPkgString = "47ccc721-c51c-40e1-bb7f-7c202cf4f49c";
        public const string guidNumberedBookmarksCmdSetString = "a279476c-595a-43da-8239-0723f2609fe8";

        public static readonly Guid guidNumberedBookmarksCmdSet = new Guid(guidNumberedBookmarksCmdSetString);
    };

    static class PkgCmdIDList
    {

        public const uint cmdOne = 0x1021;
        public const uint cmdTwo = 0x1022;
        public const uint cmdThree = 0x1023;
        public const uint cmdFour = 0x1024;
        public const uint cmdFive = 0x1025;
        public const uint cmdSix = 0x1026;
        public const uint cmdSeven = 0x1027;
        public const uint cmdEight = 0x1028;
        public const uint cmdNine = 0x1029;

        public const uint cmdGotoOne = 0x1031;
        public const uint cmdGotoTwo = 0x1032;
        public const uint cmdGotoThree = 0x1033;
        public const uint cmdGotoFour = 0x1034;
        public const uint cmdGotoFive = 0x1035;
        public const uint cmdGotoSix = 0x1036;
        public const uint cmdGotoSeven = 0x1037;
        public const uint cmdGotoEight = 0x1038;
        public const uint cmdGotoNine = 0x1039;
    };
}