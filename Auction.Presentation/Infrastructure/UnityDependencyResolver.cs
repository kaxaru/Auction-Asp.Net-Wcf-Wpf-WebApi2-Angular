using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Auction.Presentation.Infrastructure
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private IUnityContainer _unityContainer;

        public UnityDependencyResolver(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _unityContainer.Resolve(serviceType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _unityContainer.ResolveAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }
    }
}