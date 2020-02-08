import { Component, OnInit } from '@angular/core';
import { GameService } from './service';
import { Game, GameMode } from './models';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    GameMode = GameMode;

    game: Game;

    constructor(private gameService: GameService) {
    }

    async ngOnInit(): Promise<void> {
        this.game = await this.gameService.create(GameMode.MultipleChoice).toPromise();
    }
}
