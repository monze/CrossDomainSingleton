using System;
using System.Diagnostics;

namespace DomainAwareSingleton
{
    /// <summary>
    /// A domain-aware singleton. Only one instance of <typeparamref name="T"/> will exist, belonging to the default AppDomain. All members of this type are threadsafe.
    /// </summary>
    /// <typeparam name="T">The type of instance managed by this singleton.</typeparam>
    public static class Singleton<T> where T : MarshalByRefObject, new()
    {
        /// <summary>
        /// Gets the domain data key for this type. This property may be called from any AppDomain, and will return the same value regardless of AppDomain.
        /// </summary>
        private static string Name
        {
            get { return "BA9A49C7E9364060AC4E4DDDBF465684." + typeof(T).FullName; }
        }

        /// <summary>
        /// A local cache of the instance wrapper.
        /// </summary>
        private static readonly Lazy<Wrapper> LazyInstance = FindInstance();

        /// <summary>
        /// A local cache of the instance.
        /// </summary>
        private static readonly Lazy<T> CachedLazyInstance = new Lazy<T>(() => Instance);

        /// <summary>
        /// Returns a lazy that creates the instance (if necessary) and saves it in the domain data.
        /// </summary>
        private static Lazy<Wrapper> FindInstance()
        {
            return new Lazy<Wrapper>(() =>
            {
                ////string key = "ParrentAppDomainFrindlyName";
                
                // Find the upper most AppDomain, that has registrated the key.
                AppDomain currentAppDomain = AppDomain.CurrentDomain;
                AppDomain upperMostUsableDomain = currentAppDomain;
                object parentDomainFriendlyNameObject = upperMostUsableDomain.GetData(AppDomainHelper.KeyParentAppDomainFrindlyName);
                while (parentDomainFriendlyNameObject != null)
                {                    
                    ////if (parentDomainFriendlyNameObject == null)
                    ////{
                    ////    // We have found the appDoman to use, as this don't have a registreted parent AppDomain.
                    ////}
                    ////else
                    ////{
                        string desiredAppDomainFriendlyName = parentDomainFriendlyNameObject as string;

                        if (string.IsNullOrEmpty(desiredAppDomainFriendlyName))
                        {
                            string msg = string.Format("Something is wrong (and breaking the Singleton implementation) as the parent AppDoman frindly name is string.IsNullOrEmpty.");
                            Debug.Fail(msg);
                            throw new Exception(msg);
                        }

                        // Find the AppDomain, that is registreted with that friendly name.
                        AppDomain parentAppDomain = null;
                        foreach (AppDomain appDomain in AppDomainHelper.EnumerateLoadedAppDomains())
                        {
                            if (appDomain.FriendlyName.Equals(desiredAppDomainFriendlyName))
                            {
                                parentAppDomain = appDomain;
                                break;
                            }
                        }

                        if (parentAppDomain == null)
                        {
                            string msg = string.Format("Something is wrong (and breaking the Singleton implementation) as the parent AppDoman frindly name '{0}' was not located.", desiredAppDomainFriendlyName);
                            Debug.Fail(msg);
                            throw new Exception(msg);
                        }
                        else
                        {
                            // Note, we don't need to check for testHost
                            upperMostUsableDomain = parentAppDomain;
                            parentDomainFriendlyNameObject = upperMostUsableDomain.GetData(AppDomainHelper.KeyParentAppDomainFrindlyName);                          
                        }
                  
                }

                // We now have the upper most usable AppDomain. Note, this AppDomain can still be a child domain of a 
                // unitTest domain. But it should be the upper most usable domain for your code.
                Wrapper wrapper;
                if (currentAppDomain == upperMostUsableDomain)
                {
                    wrapper = new Wrapper { WrappedInstance = new T() };
                    AppDomain.CurrentDomain.SetData(Name, wrapper);
                }
                else
                {
                    //wrapper = upperMostUsableDomain.GetData(Name) as Wrapper;
                    object obj  = upperMostUsableDomain.GetData(Name);
                    wrapper = obj as Wrapper;
                    if (wrapper == null)
                    {
                        upperMostUsableDomain.DoCallBack(CreateCallback);
                        //wrapper = upperMostUsableDomain.GetData(Name) as Wrapper;

                        object obj2 = upperMostUsableDomain.GetData(Name);
                        wrapper = obj2 as Wrapper;
                    }

                }

                return wrapper;
            });
        }

        /////// <summary>
        /////// Returns a lazy that calls into the default domain to create the instance and retrieves a proxy into the current domain.
        /////// </summary>
        ////private static Lazy<Wrapper> CreateOnOtherAppDomain()
        ////{
        ////    return new Lazy<Wrapper>(() =>
        ////    {
        ////        var defaultAppDomain = AppDomainHelper.DefaultAppDomain;
        ////        var ret = defaultAppDomain.GetData(Name) as Wrapper;
        ////        if (ret != null)
        ////            return ret;
        ////        defaultAppDomain.DoCallBack(CreateCallback);
        ////        return (Wrapper)defaultAppDomain.GetData(Name);
        ////    });
        ////}

        /// <summary>
        /// Ensures the instance is created (and saved in the domain data). This method must only be called on the default AppDomain.
        /// </summary>
        private static void CreateCallback()
        {
            Wrapper wrapper = LazyInstance.Value;
        }

        /// <summary>
        /// Gets the process-wide instance. If the current domain is not the default AppDomain, this property returns a new proxy to the actual instance.
        /// </summary>
        public static T Instance { get { return LazyInstance.Value.WrappedInstance; } }

        /// <summary>
        /// Gets the process-wide instance. If the current domain is not the default AppDomain, this property returns a cached proxy to the actual instance. It is your responsibility to ensure that the cached proxy does not time out; if you don't know what this means, use <see cref="Instance"/> instead.
        /// </summary>
        public static T CachedInstance { get { return CachedLazyInstance.Value; } }

        private sealed class Wrapper : MarshalByRefObject
        {
            public override object InitializeLifetimeService()
            {
                return null;
            }

            public T WrappedInstance { get; set; }
        }
    }
}
