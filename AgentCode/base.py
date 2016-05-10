# -*- coding: utf-8 -*-
import os
from time import *

from agent import Agent


class multiagent(object):
    def __init__(self):
        self.agents = []

        for i in range(2):
            self.agents.append(Agent())

        for j in range(10):
            for x in range(50):
                self.agents[0].doStep()
            self.__writeToFile__()

    def __writeToFile__(self):
        destinationFolder = os.path.join(os.curdir, "FusionExport")
        destinationFileName = os.path.join(destinationFolder, strftime("%Y.%m.%d %H-%M-%S.fusionExport", localtime()))
        if not os.path.exists(destinationFolder):
            os.mkdir(destinationFolder)
        while os.path.exists(destinationFileName):
            return
        f = open(destinationFileName, 'w')

        for agent in self.agents:
            f.writelines(
                str.format("{0};{1};{2};{3}\r", "Agent", agent.position["x"], agent.position["y"], agent.position["z"]))

        for block in self.agents[0].blocks:
            f.writelines(
                str.format("{0};{1};{2};{3}\r", "Block", block.position["x"], block.position["y"], block.position["z"]))

        f.close()


def main():
    multiagent()


if __name__ == "__main__":
    main()
