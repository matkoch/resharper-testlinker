package com.jetbrains.rider.plugins.testlinker

import com.jetbrains.rider.actions.base.RiderAnAction
import icons.ReSharperIcons.UnitTesting

class GotoLinkedTypesWithDerivedNameAction : RiderAnAction(
        "GotoLinkedTypesWithDerivedNameAction",
        "Goto Linked Types With Derived Name",
        null,
        UnitTesting.TestFixtureToolWindow)