from pheromonetype import pheromonetype


class Pheromone(object):
    def __init__(self, intensity=50, vaporationrate=0.3, pType=pheromonetype.neutral):
        self.intensity = intensity
        self.vaporationrate = vaporationrate
        self.pheromonetype = pType
