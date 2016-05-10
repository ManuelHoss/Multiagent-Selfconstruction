class Brick(object):
    def __init__(self, size=1, position={"x": 0, "y": 0, "z": 0}):
        self.size = size
        self.position = {"x": position["x"], "y": position["y"], "z": position["z"]}
        self.pheromone = None

    def setPheromone(self, pheromone):
        self.pheromone = pheromone
