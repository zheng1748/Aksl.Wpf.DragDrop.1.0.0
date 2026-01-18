using Prism.Events;
using System;
using System.Collections;

namespace Aksl.Infrastructure.Events
{
    #region Eventbase
    public class OnBuildWorkspaceViewEventbase : PubSubEvent<OnBuildWorkspaceViewEventbase>
    {
        #region Constructors
        public OnBuildWorkspaceViewEventbase()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; }

        public MenuItem CurrentMenuItem { get; set; }
        #endregion
    }
    #endregion

    #region SideBar
    public class OnBuildHamburgerMenuSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }

    public class OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }

    public class OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }
    #endregion
}