# -*- coding: utf-8 -*-
import os
import sys
import time

sys.path.append(os.path.join(os.getcwd(), "Lib"))

from Lib.OCC import BRepPrimAPI
from Lib.aocxchange import step_ocaf
from Lib.OCC.gp import gp_Pnt
from agent import Agent



def __createBlock__(color, step_exporter, position):
    agent_box_shape = BRepPrimAPI.BRepPrimAPI_MakeBox(
        gp_Pnt(position["x"], position["y"], position["z"]), 1, 1, 1).Shape()
    if color == "red":
        step_exporter.set_color(r=1, g=0, b=0)  # red
        step_exporter.set_layer('red')
    else:
        step_exporter.set_color(r=0, g=0, b=0)  # black
        step_exporter.set_layer('black')
    step_exporter.add_shape(agent_box_shape)


class multiAgent(object):
    def __init__(self):
        self.agents = []

        for i in range(1):
            self.agents.append(Agent())

        for j in range(2):
            for x in range(300):
                for agent in self.agents:
                    agent.doStep()
            self.__exportToStepFile__()

    def __exportToStepFile__(self):
        # Create empty export file
        destinationFolder = os.path.join(os.curdir, "STEPExport")
        destinationFileName = os.path.join(destinationFolder, time.strftime("%Y.%m.%d %H-%M-%S.stp", time.localtime()))
        if not os.path.exists(destinationFolder):
            os.mkdir(destinationFolder)
        while os.path.exists(destinationFileName):
            return

        step_exporter = step_ocaf.StepOcafExporter(destinationFileName)
        for agent in self.agents:
            __createBlock__("red", step_exporter, agent.position)

        for block in self.agents[0].blocks:
            __createBlock__("black", step_exporter, block.position)

        step_exporter.write_file()


def main():
    multiAgent()


if __name__ == "__main__":
    main()
