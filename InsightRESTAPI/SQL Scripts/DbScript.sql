IF OBJECT_ID('InsightREST_GetNotes', 'P') IS NOT NULL
    DROP PROCEDURE InsightREST_GetNotes
GO
CREATE PROCEDURE [dbo].[InsightREST_GetNotes]
(
	@page INT = 1,
    @limit INT = 50,
	@user_id INT = 0,
    @note_type INT
)
AS
BEGIN
    IF @note_type = 1
	BEGIN
		SELECT N.ID as note_id, @note_type as note_type, RN.text_note as text_note FROM Notes N
		INNER JOIN RegularNotes RN
		on N.ID = RN.NoteID
		where N.note_type = @note_type and N.CreatedBy = @user_id
		ORDER BY N.ID ASC
		OFFSET @limit * (@page - 1)  ROWS
		FETCH NEXT @limit ROWS ONLY
		OPTION
		(RECOMPILE)		
	END
	ELSE IF @note_type = 2
	BEGIN
		SELECT N.ID AS note_id, @note_type AS note_type, RM.text_note AS text_note, RM.reminder_date AS reminder_date FROM Notes N
		INNER JOIN ReminderNotes RM
		on N.ID = RM.NoteID
		where N.note_type = @note_type and N.CreatedBy = @user_id
		ORDER BY N.ID ASC
		OFFSET @limit * (@page - 1)  ROWS
		FETCH NEXT @limit ROWS ONLY
		OPTION
		(RECOMPILE)	
	END
	ELSE IF @note_type = 3
	BEGIN
		SELECT N.ID AS note_id, @note_type AS note_type, TN.text_note AS text_note, TN.due_date AS due_date, TN.is_complete AS is_complete FROM Notes N
		INNER JOIN TaskNotes TN
		on N.ID = TN.NoteID
		where N.note_type = @note_type and N.CreatedBy = @user_id
		ORDER BY N.ID ASC
		OFFSET @limit * (@page - 1)  ROWS
		FETCH NEXT @limit ROWS ONLY
		OPTION
		(RECOMPILE)	
	END
	ELSE IF @note_type = 4
	BEGIN
		SELECT N.ID AS note_id, @note_type AS note_type, B.url AS url FROM Notes N
		INNER JOIN BookmarkNotes B
		on N.ID = B.NoteID
		where N.note_type = @note_type and N.CreatedBy = @user_id
		ORDER BY N.ID ASC
		OFFSET @limit * (@page - 1)  ROWS
		FETCH NEXT @limit ROWS ONLY
		OPTION
		(RECOMPILE)	
	END
END
GO