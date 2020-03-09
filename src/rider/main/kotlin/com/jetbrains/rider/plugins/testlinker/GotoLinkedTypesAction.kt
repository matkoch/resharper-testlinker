package com.jetbrains.rider.plugins.testlinker

import com.jetbrains.rider.actions.base.RiderAnAction
import icons.ReSharperIcons.UnitTesting

class GotoLinkedTypesAction : RiderAnAction(
        "GotoLinkedTypesAction",
        "Goto Linked Types",
        null,
        UnitTesting.TestFixtureToolWindow)