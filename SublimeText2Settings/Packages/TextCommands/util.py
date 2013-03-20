"""
Copyright (c) 2013 Samuel B. Fries

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
"""

import re

"""
    Common functions used by various classes
"""


def string_as_integer(string):
    reg = re.compile("-?[0-9]+")
    match = reg.match(string)
    if match is not None:
        return int(string[match.start():match.end()])
    else:
        return None


def num_to_hex(num):
    hex = "0123456789ABCDEF"

    val = num
    hexstr = ""

    max_pow = 0
    while 16 ** max_pow < val:
        max_pow += 1

    while max_pow > 0:
        max_pow -= 1
        hex_dig = val // 16 ** max_pow
        val = val % 16 ** max_pow
        hexstr += hex[hex_dig]

    return hexstr
