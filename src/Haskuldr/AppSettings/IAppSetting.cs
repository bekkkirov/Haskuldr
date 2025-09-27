namespace Haskuldr.AppSettings;

public interface IAppSetting
{
    static abstract string Section { get; }
}