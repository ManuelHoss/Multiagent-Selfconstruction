from enum import Enum


class pheromonetype(Enum):
    initial = 1
    neutral = 2

    class build(Enum):
        front = 1
        back = 2
        left = 3
        right = 4
        up = 5
        down = 6
