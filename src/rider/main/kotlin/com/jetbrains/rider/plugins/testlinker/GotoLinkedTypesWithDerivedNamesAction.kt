package com.jetbrains.rider.plugins.testlinker

import com.jetbrains.rider.actions.base.RiderAnAction
import com.jetbrains.rider.icons.ReSharperUnitTestingIcons

class GotoLinkedTypesWithDerivedNamesAction : RiderAnAction(
        "Goto_LinkedTypesWithDerivedNames",
        "Goto Linked Types With Derived Names",
        null,
        ReSharperUnitTestingIcons.TestFixtureToolWindow)