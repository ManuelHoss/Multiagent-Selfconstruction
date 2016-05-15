from enum import Enum


class PheromoneType(Enum):
    initial = 1
    neutral = 2
    build = 3

    # TODO: used later to extend functionality
    class Build(Enum):
        front = 1
        back = 2
        left = 3
        right = 4
        up = 5
        down = 6