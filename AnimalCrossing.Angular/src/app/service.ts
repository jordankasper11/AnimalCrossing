import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Game, GameMode, GuessRequest, GuessResponse, SkipRequest } from './models';

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

    guess(request: GuessRequest): Observable<GuessResponse> {
        const requestUrl = '/api/Game/Guess';

        return this.httpClient.post<GuessResponse>(requestUrl, request);
    }

    skip(request: SkipRequest): Observable<Game> {
        const requestUrl = '/api/Game/Skip';

        return this.httpClient.post<Game>(requestUrl, request);
    }
}