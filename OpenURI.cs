// I cannot take credit for this.
// This was quickly set up by somebody on Stack Overflow --> https://stackoverflow.com/questions/502199/how-to-open-a-web-page-from-my-application
// The user who was so kind to come up with this: https://stackoverflow.com/users/184528/cdiggins ;)

using System.Diagnostics;
using System;

namespace TrickyUnits
{
    class OURI
    {
        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        public static bool OpenUri(string uri)
        {
            if (!IsValidUri(uri))
                return false;
            Process.Start(uri);
            return true;
        }

    }
}
