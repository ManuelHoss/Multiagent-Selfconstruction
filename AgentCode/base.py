# -*- coding: utf-8 -*-
import os
from time import *

import OCC.BRepPrimAPI
import aocxchange.step_ocaf
from OCC.gp import gp_Pnt
from agent import Agent


class multiagent(object):
    def __init__(self):
        self.agents = []

        for i in range(3):
            self.agents.append(Agent())

        for j in range(3):
            for x in range(50):
                for agent in self.agents:
                    agent.doStep()
            self.__exportToStepFile__()

    def __exportToStepFile__(self):
        # Create empty export file
        destinationFolder = os.path.join(os.curdir, "STEPExport")
        destinationFileName = os.path.join(destinationFolder, strftime("%Y.%m.%d %H-%M-%S.stp", localtime()))
        if not os.path.exists(destinationFolder):
            os.mkdir(destinationFolder)
        while os.path.exists(destinationFileName):
            return

        step_exporter = aocxchange.step_ocaf.StepOcafExporter(destinationFileName)
        for agent in self.agents:
            # Create Box at Position agent.position with dimensions 10mm x 10mm x 10mm
            agent_box_shape = OCC.BRepPrimAPI.BRepPrimAPI_MakeBox(
                gp_Pnt(agent.position["x"], agent.position["y"], agent.position["z"]), 1, 1, 1).Shape()
            step_exporter.set_color(r=1, g=0, b=0)  # red
            step_exporter.set_layer('red')
            step_exporter.add_shape(agent_box_shape)

        for block in self.agents[0].blocks:
            # Create Box at Position block.position with dimensions 10mm x 10mm x 10mm
            block_box_shape = OCC.BRepPrimAPI.BRepPrimAPI_MakeBox(
                gp_Pnt(block.position["x"], block.position["y"], block.position["z"]), 1, 1, 1).Shape()
            step_exporter.set_color(r=0, g=0, b=0)  # black
            step_exporter.set_layer('black')
            step_exporter.add_shape(block_box_shape)

        step_exporter.write_file()


def main():
    multiagent()


if __name__ == "__main__":
    main()
