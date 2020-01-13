using System;
using Windows.UI;

namespace Madeline.Frontend
{
    // https://tailwindcss.com/docs/customizing-colors#default-color-palette
    internal static class Palette
    {
        public static Color FromHex(uint code)
        {
            byte[] bytes = BitConverter.GetBytes(code);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static Color Black = FromHex(0xFF000000);
        public static Color White = FromHex(0xFFFFFFFF);

        public static Color Tone1 = FromHex(0xFF101010);
        public static Color Tone2 = FromHex(0xFF201010);
        public static Color Tone3 = FromHex(0xFF303030);
        public static Color Tone4 = FromHex(0xFF404040);
        public static Color Tone5 = FromHex(0xFF505050);
        public static Color Tone6 = FromHex(0xFF606060);
        public static Color Tone7 = FromHex(0xFF707070);
        public static Color Tone8 = FromHex(0xFF808080);
        public static Color Tone9 = FromHex(0xFF909090);
        public static Color ToneA = FromHex(0xFFA0A0A0);
        public static Color ToneB = FromHex(0xFFB0B0B0);
        public static Color ToneC = FromHex(0xFFC0C0C0);
        public static Color ToneD = FromHex(0xFFD0D0D0);
        public static Color ToneE = FromHex(0xFFE0E0E0);
        public static Color ToneF = FromHex(0xFFF0F0F0);

        public static Color Gray1 = FromHex(0xFFF7FAFC);
        public static Color Gray2 = FromHex(0xFFEDF2F7);
        public static Color Gray3 = FromHex(0xFFE2E8F0);
        public static Color Gray4 = FromHex(0xFFCBD5E0);
        public static Color Gray5 = FromHex(0xFFA0AEC0);
        public static Color Gray6 = FromHex(0xFF718096);
        public static Color Gray7 = FromHex(0xFF4A5568);
        public static Color Gray8 = FromHex(0xFF2D3748);
        public static Color Gray9 = FromHex(0xFF1A202C);

        public static Color Red1 = FromHex(0xFFFFF5F5);
        public static Color Red2 = FromHex(0xFFFED7D7);
        public static Color Red3 = FromHex(0xFFFEB2B2);
        public static Color Red4 = FromHex(0xFFFC8181);
        public static Color Red5 = FromHex(0xFFF56565);
        public static Color Red6 = FromHex(0xFFE53E3E);
        public static Color Red7 = FromHex(0xFFC53030);
        public static Color Red8 = FromHex(0xFF9B2C2C);
        public static Color Red9 = FromHex(0xFF742A2A);

        public static Color Orange1 = FromHex(0xFFFFFAF0);
        public static Color Orange2 = FromHex(0xFFFEEBC8);
        public static Color Orange3 = FromHex(0xFFFBD38D);
        public static Color Orange4 = FromHex(0xFFF6AD55);
        public static Color Orange5 = FromHex(0xFFED8936);
        public static Color Orange6 = FromHex(0xFFDD6B20);
        public static Color Orange7 = FromHex(0xFFC05621);
        public static Color Orange8 = FromHex(0xFF9C4221);
        public static Color Orange9 = FromHex(0xFF7B341E);

        public static Color Yellow1 = FromHex(0xFFFFFFF0);
        public static Color Yellow2 = FromHex(0xFFFEFCBF);
        public static Color Yellow3 = FromHex(0xFFFAF089);
        public static Color Yellow4 = FromHex(0xFFF6E05E);
        public static Color Yellow5 = FromHex(0xFFECC94B);
        public static Color Yellow6 = FromHex(0xFFD69E2E);
        public static Color Yellow7 = FromHex(0xFFB7791F);
        public static Color Yellow8 = FromHex(0xFF975A16);
        public static Color Yellow9 = FromHex(0xFF744210);

        public static Color Green1 = FromHex(0xFFF0FFF4);
        public static Color Green2 = FromHex(0xFFC6F6D5);
        public static Color Green3 = FromHex(0xFF9AE6B4);
        public static Color Green4 = FromHex(0xFF68D391);
        public static Color Green5 = FromHex(0xFF48BB78);
        public static Color Green6 = FromHex(0xFF38A169);
        public static Color Green7 = FromHex(0xFF2F855A);
        public static Color Green8 = FromHex(0xFF276749);
        public static Color Green9 = FromHex(0xFF22543D);

        public static Color Teal1 = FromHex(0xFFE6FFFA);
        public static Color Teal2 = FromHex(0xFFB2F5EA);
        public static Color Teal3 = FromHex(0xFF81E6D9);
        public static Color Teal4 = FromHex(0xFF4FD1C5);
        public static Color Teal5 = FromHex(0xFF38B2AC);
        public static Color Teal6 = FromHex(0xFF319795);
        public static Color Teal7 = FromHex(0xFF2C7A7B);
        public static Color Teal8 = FromHex(0xFF285E61);
        public static Color Teal9 = FromHex(0xFF234E52);

        public static Color Blue1 = FromHex(0xFFEBF8FF);
        public static Color Blue2 = FromHex(0xFFBEE3F8);
        public static Color Blue3 = FromHex(0xFF90CDF4);
        public static Color Blue4 = FromHex(0xFF63B3ED);
        public static Color Blue5 = FromHex(0xFF4299E1);
        public static Color Blue6 = FromHex(0xFF3182CE);
        public static Color Blue7 = FromHex(0xFF2B6CB0);
        public static Color Blue8 = FromHex(0xFF2C5282);
        public static Color Blue9 = FromHex(0xFF2A4365);

        public static Color Indigo1 = FromHex(0xFFEBF4FF);
        public static Color Indigo2 = FromHex(0xFFC3DAFE);
        public static Color Indigo3 = FromHex(0xFFA3BFFA);
        public static Color Indigo4 = FromHex(0xFF7F9CF5);
        public static Color Indigo5 = FromHex(0xFF667EEA);
        public static Color Indigo6 = FromHex(0xFF5A67D8);
        public static Color Indigo7 = FromHex(0xFF4C51BF);
        public static Color Indigo8 = FromHex(0xFF434190);
        public static Color Indigo9 = FromHex(0xFF3C366B);

        public static Color Purple1 = FromHex(0xFFFAF5FF);
        public static Color Purple2 = FromHex(0xFFE9D8FD);
        public static Color Purple3 = FromHex(0xFFD6BCFA);
        public static Color Purple4 = FromHex(0xFFB794F4);
        public static Color Purple5 = FromHex(0xFF9F7AEA);
        public static Color Purple6 = FromHex(0xFF805AD5);
        public static Color Purple7 = FromHex(0xFF6B46C1);
        public static Color Purple8 = FromHex(0xFF553C9A);
        public static Color Purple9 = FromHex(0xFF44337A);

        public static Color Pink1 = FromHex(0xFFFFF5F7);
        public static Color Pink2 = FromHex(0xFFFED7E2);
        public static Color Pink3 = FromHex(0xFFFBB6CE);
        public static Color Pink4 = FromHex(0xFFF687B3);
        public static Color Pink5 = FromHex(0xFFED64A6);
        public static Color Pink6 = FromHex(0xFFD53F8C);
        public static Color Pink7 = FromHex(0xFFB83280);
        public static Color Pink8 = FromHex(0xFF97266D);
        public static Color Pink9 = FromHex(0xFF702459);
    }
}
