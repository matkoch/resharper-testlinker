package com.jetbrains.rider.plugins.testlinker

import com.jetbrains.rider.actions.base.RiderAnAction
import com.jetbrains.rider.icons.ReSharperUnitTestingIcons

class GotoLinkedTypesAction : RiderAnAction(
        "GotoLinkedTypesAction",
        "Goto Linked Types",
        null,
        ReSharperUnitTestingIcons.TestFixtureToolWindow)