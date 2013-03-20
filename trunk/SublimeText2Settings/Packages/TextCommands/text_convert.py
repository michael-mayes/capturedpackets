"""
Copyright (c) 2013 Samuel B. Fries

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
"""

import sublime
import sublime_plugin


class SnakeToCamelCase(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        for region in regions:
            val = view.substr(region)
            strings = [x.capitalize() for x in val.split("_")[1:]]
            val = val.split("_")[0] + "".join(strings)
            view.replace(edit, region, val)


class UnderscoreToSpacesCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        for region in regions:

            val = view.substr(region)

            val = " ".join(val.split("_"))

            self.view.replace(edit, region, val)


class SpacesToUnderscoresCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        for region in regions:
            val = view.substr(region)
            val = "_".join(val.split(" "))
            self.view.replace(edit, region, val)
