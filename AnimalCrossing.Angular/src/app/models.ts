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
    public correctGuesses: number;
    public wrongGuesses: number;
    public skips: number;
    public remaining: number;
    public previousVillager: Villager;
    public currentVillager: CurrentVillager;
    public options: Array<VillagerOption>;
}

export class GuessRequest {
    constructor(public gameId: string, public name: string) {
    }
}

export class GuessResponse {
    public success: boolean;
    public game: Game;
}

export class SkipRequest {
    constructor(public gameId: string) {
    }
}