let roles = {
    "Member" : 0,
    "Technician" : 100,
    "Head Technician" : 200,
    "Owner" : 300
}

export function hasPerm(user, minRole) {
    return user.role !== null && roles[user.role] >= roles[minRole];
}