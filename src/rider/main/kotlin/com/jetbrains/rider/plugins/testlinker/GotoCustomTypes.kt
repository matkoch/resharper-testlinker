package com.jetbrains.rider.plugins.testlinker

import com.jetbrains.rider.actions.base.RiderAnAction
import com.jetbrains.rider.icons.ReSharperUnitTestingIcons

class GotoCustomTypesAction : RiderAnAction(
        "NavigateToLinkedTypes",
        "Goto Custom Types",
        null,
        ReSharperUnitTestingIcons.TestFixtureToolWindow)