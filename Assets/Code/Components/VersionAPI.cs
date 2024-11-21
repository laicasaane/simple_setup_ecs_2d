namespace SimpleSetupEcs2d
{
    public static class VersionAPI
    {
        public const byte VERSION_1 = 0;
        public const byte VERSION_2 = 1;

        public static byte GetVersion(bool version2)
            => version2 ? VERSION_2 : VERSION_1;
    }
}
