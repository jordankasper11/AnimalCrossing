export class Villager {
    public id: string;
    public name: string;
    public url: string;
}

export class CurrentVillager {
    public id: string;
    public houseImageUrl: string;
}

export class VillagerOption {
    public id: string;
    public name: string;
}

export enum GameMode {
    Guess,
    MultipleChoice
}

export class Game {
    public id: string;
    public mode: GameMode;
    public completed: boolean;
    public wrongGuesses: number;
    public skips: number;
    public currentVillager: CurrentVillager;
    public options: Array<VillagerOption>;
}
