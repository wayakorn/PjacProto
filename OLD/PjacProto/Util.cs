using System;
using Windows.Storage;

namespace PjacProto
{
    static class Util
    {
        public static string ReadTextAsset(string fileName)
        {
            var assetUri = new Uri(string.Format("ms-appx:///Assets/{0}", fileName));
            StorageFile anjFile = StorageFile.GetFileFromApplicationUriAsync(assetUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            string result = FileIO.ReadTextAsync(anjFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            return result;
        }
    }
}
