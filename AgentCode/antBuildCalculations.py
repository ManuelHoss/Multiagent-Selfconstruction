import utils


def calculatePheromoneIntensity(pheromones, agentPosition):
    pheromoneIntesity = 0
    for pheromone in pheromones:
        distance = utils.calculateDistanceToBrick(agentPosition, pheromone.position)
        pheromoneIntesity += pheromone.getIntensity() / (
        distance * distance) if distance != 0 else pheromone.getIntensity()
    return True if pheromoneIntesity >= 0.05 else False


def getSortedInfluences(pheromones, agentPosition):
    mostInfluental = []
    for pheromone in pheromones:
        distance = utils.calculateDistanceToBrick(agentPosition, pheromone.position)
        pheromoneIntesity = pheromone.getIntensity() / (
        distance * distance) if distance != 0 else pheromone.getIntensity()
        mostInfluental.append((pheromone, pheromoneIntesity))

    return sorted(mostInfluental, key=lambda intensity: intensity[1], reverse=True)


def calculateAndMove(agentPosition, pheromone):
    for key in agentPosition:
        if agentPosition[key] < pheromone[key]:
            agentPosition[key] += 1
        elif agentPosition[key] > pheromone[key]:
            agentPosition[key] -= 1
    return agentPosition
