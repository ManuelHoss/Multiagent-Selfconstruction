import random

from buildingshape import Brick


class Agent(object):
    blocks = []

    def __init__(self):
        self.payload = None
        self.position = {"x": random.randrange(start=0, stop=500),
                         "y": random.randrange(start=0, stop=500),
                         "z": random.randrange(start=0, stop=500)}

    def doStep(self):
        self.__build__()
        self.__move__()

    def __evaluateNeighbours__(self):
        pass

    def __build__(self):
        self.blocks.append(Brick(position=self.position))

    def __remove__(self):
        pass

    def __move__(self):
        nextStep = random.randrange(start=0, stop=26)

        # Move in Position X
        if nextStep in [0, 3, 6, 10, 13, 16, 20, 23, 26]:
            self.position["x"] += 1

        if nextStep in [2, 5, 8, 12, 15, 18, 22, 25, 28]:
            self.position["y"] -= 1

        # Move in Position Y
        if nextStep in range(8):
            self.position["y"] += 1

        if nextStep in range(20, 28):
            self.position["y"] -= 1

        # Move in Position Z
        if nextStep in [0, 1, 2, 10, 11, 12, 20, 21, 22]:
            self.position["z"] += 1

        if nextStep in [6, 7, 8, 16, 17, 18, 26, 27, 28]:
            self.position["z"] -= 1

    def addPayload(self, pheromone):
        self.payload = pheromone
