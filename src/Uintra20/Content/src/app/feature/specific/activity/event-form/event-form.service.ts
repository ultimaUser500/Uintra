import { Injectable } from '@angular/core';
import { ITagData } from 'src/app/feature/reusable/inputs/tag-multiselect/tag-multiselect.interface';
import { ISelectItem } from 'src/app/feature/reusable/inputs/select/select.component';
import { IOwner } from '../activity.interfaces';
import { IEventCreateModel } from './event-form.interface';

@Injectable({
  providedIn: 'root'
})
export class EventFormService {

  constructor() { }

  getEventDataInitialValue(data: any): IEventCreateModel {
    return {
      ownerId: data.creator.id,
      title: data.title || "",
      description: data.description || "",
      tagIdsData: data.availableTags,
      publishDate: data.publishDate || undefined,
      startDate: data.startDate || undefined,
      endDate: data.endDate || undefined,
      locationTitle: data.locationTitle || "",
      location: {
        address: (data.location && data.location.address) || null,
        shortAddress:(data.location && data.location.shortAddress) || null
      },
      canSubscribe: data.canSubscribe || false,
      subscribeNotes: data.subscribeNotes || "",
      pinAllowed: data.pinAllowed,
      isPinned: data.isPinned || false,
      endPinDate: data.endPinDate || null,
      newMedia: "",
      media: data.lightboxPreviewModel || null,
      groupId: data.groupId || null
    };
  }
  getTagsForResponse(selectedTags: ITagData[] = []): string[] {
    return selectedTags.map(tag => tag.id);
  }
  getMediaIdsForResponse(files: Array<any>): string {
    return files.map(file => file[1]).join(",");
  }
  getMembers(members = []): ISelectItem[] {
    return members.map(member => ({
      id: member.id,
      text: member.displayedName
    }));
  }
}