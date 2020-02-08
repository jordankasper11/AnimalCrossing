import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Game, GameMode } from './models';

@Injectable({
    providedIn: 'root'
})
export class GameService {
    constructor(private httpClient: HttpClient) {
    }

    create(mode: GameMode) {
        const requestUrl = `/api/Game?mode=${mode}`;

        return this.httpClient.get<Game>(requestUrl);
    }

    get(id: string): Observable<Game> {
        const requestUrl = '/api/Game/${id}';

        return this.httpClient.get<Game>(requestUrl);
    }
}