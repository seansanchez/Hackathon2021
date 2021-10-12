import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http'
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { IScavengerHunt } from "../models/IScavengerHunt";

/**
 * A service to interface with the API
 */
@Injectable({
    providedIn: "root"
})
export class ApiService {

    private _apiUrl = environment.apiUrl;

    constructor(private readonly http: HttpClient) {}

    /** Gets a Scavenger Hunt with a specified game code */
    public getScavengerHunt(gameCode: string): Observable<IScavengerHunt> {
        return this.http.get<IScavengerHunt>(`${this._apiUrl}/api/scavenger-hunt/${gameCode}`);
    }
}
