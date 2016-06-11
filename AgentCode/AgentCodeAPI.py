import threading

from agent import Agent


class api(object):
    def __init__(self):
        self.agents = []

    def runAgentsAndRetrive(self, numberOfAgents=5, numberOfSteps=50):
        for i in range(numberOfAgents):
            self.agents.append(Agent())
        for x in range(numberOfSteps):
            threads = []
            for agent in self.agents:
                t = threading.Thread(target=agent.doStep)
                threads += [t]
                t.start()
            for t in threads:
                t.join()
            print "loop:" + str(x)
            self.agents[0].removeVaporatedPheromones()

        return self.agents
