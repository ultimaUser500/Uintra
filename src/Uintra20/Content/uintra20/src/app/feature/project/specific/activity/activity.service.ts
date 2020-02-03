import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Subject} from "rxjs";
import {
  ISocialCreateModel,
  INewsCreateModel,
  ISocialEdit
} from "./activity.interfaces";

@Injectable({
  providedIn: "root"
})
export class ActivityService {
  private feedRefreshTrigger = new Subject();
  feedRefreshTrigger$ = this.feedRefreshTrigger.asObservable();

  constructor(private http: HttpClient) {}

  submitSocialContent(data: ISocialCreateModel) {
    return this.http
      .post("/ubaseline/api/social/createExtended", data)
      .toPromise();
  }
  updateSocial(model: ISocialEdit) {
    return this.http.put("/ubaseline/api/social/Update", model);
  }

  public deleteSocial(id: string) {
    return this.http.delete("/ubaseline/api/social/Delete", {
      params: new HttpParams().set("id", id)
    });
  }

  submitNewsContent(data: INewsCreateModel) {
    //TODO Interface for type
    return this.http.post("/ubaseline/api/newsApi/create", data);
  }

  refreshFeed() {
    this.feedRefreshTrigger.next();
  }
}