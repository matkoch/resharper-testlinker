package com.jetbrains.rider.plugins.testlinker.options

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class TestLinkerOptionsPage : SimpleOptionsPage("TestLinker", "TestLinkerOptionsPage") {
    override fun getId(): String {
        return "TestLinkerOptionsPage"
    }
}