package com.jetbrains.rider.plugins.testlinker

import com.jetbrains.rider.actions.base.RiderAnAction
import com.jetbrains.rider.icons.ReSharperUnitTestingIcons

class GotoLinkedTypesWithDerivedNameAction : RiderAnAction(
        "GotoLinkedTypesWithDerivedNameAction",
        "Goto Linked Types With Derived Name",
        null,
        ReSharperUnitTestingIcons.TestFixtureToolWindow)