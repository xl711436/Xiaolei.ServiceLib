using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Xiaolei.ServiceLib
{
    /// <summary>  
    /// Windows服务辅助类  
    /// </summary>  
    public class ServiceHelper
    {
        /// <summary>  
        /// 检查服务是否存在
        /// </summary>  
        /// <param name="nameService">服务名</param>  
        /// <returns>存在返回 true,否则返回 false;</returns>  
        public static bool IsServiceExisted(string nameService)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName.ToLower() == nameService.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>  
        ///     安装Windows服务  
        /// </summary>  
        /// <param name="filepath">程序文件路径</param>  
        public static void InstallService(string filepath)
        {
            IDictionary stateSaver = new Hashtable();
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = filepath;
                stateSaver.Clear();
                installer.Install(stateSaver);
                installer.Commit(stateSaver);
            }
        }

        /// <summary>  
        ///     卸载Windows服务  
        /// </summary>  
        /// <param name="filepath">程序文件路径</param>  
        public static void UnInstallService(string filepath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = filepath;
                installer.Uninstall(null);
            }
        }

        /// <summary>  
        ///     检查Windows服务是否在运行  
        /// </summary>  
        /// <param name="serviceName">程序的服务名</param>  
        public static bool IsRunning(string serviceName)
        {
            bool isRun = false;
            try
            {
                if (!IsServiceExisted(serviceName))
                {
                    return false;
                }
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status == ServiceControllerStatus.StartPending ||
                       sc.Status == ServiceControllerStatus.Running)
                    {
                        isRun = true;
                    }
                }
            }
            catch
            {
                isRun = false;
            }
            return isRun;
        }

        /// <summary>  
        ///     启动Windows服务  
        /// </summary>  
        /// <param name="serviceName">程序的服务名</param>  
        /// <returns>启动成功返回 true,否则返回 false;</returns>  
        public static bool StartService(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Stopped || sc.Status == ServiceControllerStatus.StopPending)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));
                }
            }

            return true;
        }

        /// <summary>  
        ///     停止Windows服务  
        /// </summary>  
        /// <param name="serviceName">程序的服务名</param>  
        /// <returns>停止成功返回 true,否则返回 false;</returns>  
        public static bool StopService(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 10));
                }
            }

            return true;
        }

        /// <summary>  
        ///     重启Windows服务  
        /// </summary>  
        /// <param name="serviceName">程序的服务名</param>  
        /// <returns>重启成功返回 true,否则返回 false;</returns>  
        public static bool RefreshService(string serviceName)
        {
            return StopService(serviceName) && StartService(serviceName);
        }
    }


}
